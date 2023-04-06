namespace NDetours;

using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using PInvoke;

#if WINDOWS10_0_17763_0_OR_GREATER
using Windows.System;
#endif

public static class ProcessDetour {
    public static unsafe Process Start(string exe,
                                       IReadOnlyDictionary<string, string>? env,
                                       Kernel32.CreateProcessFlags flags,
                                       string[] injectDlls) {
        var startupInfo = new Kernel32.STARTUPINFO {
            cb = sizeof(Kernel32.STARTUPINFO),
        };
        Kernel32.PROCESS_INFORMATION processInfo;
        var dlls = new StrPtr[injectDlls.Length];
        byte*[] dllsMarked = new byte*[dlls.Length + 1];
        try {
            for (int i = 0; i < injectDlls.Length; i++) {
                dlls[i] = new(injectDlls[i], Encoding.ASCII);
                dllsMarked[i] = (byte*)dlls[i].RawPointer;
            }

            flags |= Kernel32.CreateProcessFlags.CREATE_UNICODE_ENVIRONMENT;
            char[]? envBlock = MakeEnvironmentBlock(env);

            if (!Detour.CreateProcessWithDlls(
                    exe,
                    commandLine: null,
                    processAttributes: null, threadAttributes: null,
                    inheritHandles: true,
                    flags: flags,
                    lpEnvironment: envBlock,
                    currentDirectory: null,
                    ref startupInfo,
                    out processInfo,
                    injectDlls.Length,
                    dllsMarked,
                    IntPtr.Zero))
                throw new Win32Exception();
        } finally {
            foreach (var dll in dlls) dll.Dispose();
        }

        return Process.GetProcessById(processInfo.dwProcessId);
    }

    /// <summary>
    /// Launches UWP app and injects specified DLLs into it.
    /// </summary>
    /// <param name="debugPause">Allows debugging injection process by waiting for 30 seconds,
    /// which gives time to attach debugger to <c>NDetours.exe</c></param>
    /// <returns>The newly launched process</returns>
    /// <exception cref="FileNotFoundException">
    /// <para>App package specified in <paramref name="packageFullName"/> is not installed.</para>
    /// <para>-or-</para>
    /// <para><paramref name="appUserModelID"/> does not refer to a known app.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">The app was already running</exception>
#if WINDOWS10_0_19041_0_OR_GREATER
    [SupportedOSPlatform("windows10.0.19041.0")]
    public static async Task<Process> Start(string appUserModelID,
                                            string packageFullName,
                                            string? arguments,
                                            IReadOnlyDictionary<string, string>? env,
                                            string[] injectDlls,
                                            bool debugPause = false) {
        string commandLine = $"\"{CommandLine.ExePath}\" inject"
                           + $" \"--dlls={string.Join(Path.PathSeparator, injectDlls)}\""
                           + " --resume";
        if (debugPause)
            commandLine += " --debug";

        char[]? environment = MakeEnvironmentBlock(env);

        var appDiagnosticInfos = await AppDiagnosticInfo.RequestInfoForAppAsync(appUserModelID)
                                                        .AsTask().ConfigureAwait(false);

        var appInfo =
            appDiagnosticInfos.FirstOrDefault(
                info => info.AppInfo.Package.Id.FullName == packageFullName)
         ?? throw new FileNotFoundException("Package not found", fileName: packageFullName);

        foreach (var resourceGroup in appInfo.GetResourceGroups()) {
            var groupState = resourceGroup.GetStateReport();
            if (groupState.ExecutionState is AppResourceGroupExecutionState.Running
                or AppResourceGroupExecutionState.Suspending) {
                throw new InvalidOperationException("App is already running");
            }
        }

        var debugSettings = PackageDebug.CreateSettings();

        debugSettings.DisableDebugging(packageFullName).ThrowOnFailure();
        debugSettings.TerminateAllProcesses(packageFullName).ThrowOnFailure();

        debugSettings.EnableDebugging(packageFullName: packageFullName,
                                      debuggerCommandLine: commandLine,
                                      environment: environment)
                     .ThrowOnFailure();

        int processID;
        try {
            var activator = new ApplicationActivationManager();
            activator.ActivateApplication(appUserModelId: appUserModelID, arguments: arguments,
                                          ActivateOptions.None, out processID)
                     .ThrowOnFailure();
        } finally {
            debugSettings.DisableDebugging(packageFullName).ThrowOnFailure();
        }

        return Process.GetProcessById(processID);
    }
#else
    public static unsafe Process Start(string appUserModelID, string packageFullName,
                                       string? arguments, string[] injectDlls,
                                       bool debugPause = false) {
        throw new PlatformNotSupportedException();
    }
#endif

    static char[]? MakeEnvironmentBlock(IReadOnlyDictionary<string, string>? env) {
        if (env is null) return null;

        var sb = new StringBuilder();
        foreach (var kv in env) {
            if (kv.Key.Contains('='))
                throw new ArgumentException("Environment variable name cannot contain '='",
                                            nameof(env));
            sb.Append(kv.Key);
            sb.Append('=');
            sb.Append(kv.Value);
            sb.Append('\0');
        }
        sb.Append('\0');
        return sb.ToString().ToCharArray();
    }
}