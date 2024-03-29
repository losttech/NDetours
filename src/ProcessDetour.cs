﻿namespace NDetours;

using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using PInvoke;

#if WINDOWS10_0_17763_0_OR_GREATER
using Windows.System;
#endif

public static partial class ProcessDetour {
    public static Process Start(string exe,
                                IReadOnlyDictionary<string, string>? env,
                                Kernel32.CreateProcessFlags flags,
                                string[] injectDlls) {
        var startInfo = new StartInfo(exe) {
            Flags = flags,
        };
        if (env is not null) {
            startInfo.Environment = new Dictionary<string, string>();
            foreach (var kv in env)
                startInfo.Environment[kv.Key] = kv.Value;
        }
        foreach (string dll in injectDlls ?? throw new ArgumentNullException(nameof(injectDlls))) {
            startInfo.InjectDlls.Add(dll);
        }
        return Start(startInfo);
    }

    public static unsafe Process Start(StartInfo startInfo) {
        var startupInfo = new Kernel32.STARTUPINFO {
            cb = sizeof(Kernel32.STARTUPINFO),
        };
        if (startInfo.WindowStyle is { } style && style != ProcessWindowStyle.Normal) {
            startupInfo.dwFlags |= Kernel32.StartupInfoFlags.STARTF_USESHOWWINDOW;
            startupInfo.wShowWindow = style switch {
                ProcessWindowStyle.Hidden => ShowWindow.HIDE,
                ProcessWindowStyle.Minimized => ShowWindow.MINIMIZE,
                ProcessWindowStyle.Maximized => ShowWindow.MAXIMIZE,
                _ => throw new ArgumentOutOfRangeException(nameof(startInfo.WindowStyle)),
            };
        }
        Kernel32.PROCESS_INFORMATION processInfo;
        var flags = startInfo.Flags;
        var dlls = new StrPtr[startInfo.InjectDlls.Count];
        byte*[] dllsMarked = new byte*[dlls.Length + 1];
        try {
            for (int i = 0; i < startInfo.InjectDlls.Count; i++) {
                string unicodePath = startInfo.InjectDlls[i];
                string compatiblePath =
                    unicodePath.Any(c => c > 127)
                        ? UnicodePaths.GetShortPath(unicodePath)
                        : unicodePath;
                Debug.WriteLine(compatiblePath);
                dlls[i] = new(compatiblePath, Encoding.ASCII);
                dllsMarked[i] = (byte*)dlls[i].RawPointer;
            }

            flags |= Kernel32.CreateProcessFlags.CREATE_UNICODE_ENVIRONMENT;
            char[]? envBlock = MakeEnvironmentBlock(startInfo.Environment);

            if (!Detour.CreateProcessWithDlls(
                    startInfo.Executable,
                    commandLine: startInfo.CommandLine,
                    processAttributes: null, threadAttributes: null,
                    inheritHandles: true,
                    flags: flags,
                    lpEnvironment: envBlock,
                    currentDirectory: startInfo.WorkingDirectory,
                    ref startupInfo,
                    out processInfo,
                    startInfo.InjectDlls.Count,
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
    /// <param name="launcherExe">Path to the executable, that will be called to
    /// inject DLLs.
    /// <para>The simplest way to set is to point this parameter back to your own executable,
    /// and make it handle the <c>inject</c> command.
    /// Use <see cref="InjectCommand"/> to do it via
    /// <see href="https://github.com/fschwiet/ManyConsole/#quick-start-guide">ManyConsole</see>.
    /// </para></param>
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
                                            string launcherExe,
                                            string[] injectDlls,
                                            bool debugPause = false) {
        string commandLine = $"\"{launcherExe}\" inject --resume";

        string listPath = Path.GetTempFileName();
        try {
            using var dllsFile = new StreamWriter(listPath, new FileStreamOptions {
                Access = FileAccess.Write,
                Mode = FileMode.Append,
                Share = FileShare.Read,
            });
            foreach (string dll in injectDlls)
                await dllsFile.WriteLineAsync(dll).ConfigureAwait(false);
            await dllsFile.FlushAsync().ConfigureAwait(false);

            commandLine += $" -list \"{listPath}\"";

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

            EnableDebugging(debugSettings,
                            packageFullName: packageFullName,
                            debuggerCommandLine: commandLine,
                            environment: environment);

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
        } finally {
            if (File.Exists(listPath))
                File.Delete(listPath);
        }
    }
#else
    public static unsafe Process Start(string appUserModelID, string packageFullName,
                                       string? arguments, string[] injectDlls,
                                       bool debugPause = false) {
        throw new PlatformNotSupportedException();
    }
#endif

    static unsafe void EnableDebugging(IPackageDebugSettings debugSettings,
                                       string packageFullName, string debuggerCommandLine,
                                       char[]? environment) {
        if (debuggerCommandLine.Length > 255)
            throw new ArgumentOutOfRangeException(nameof(debuggerCommandLine), "Too long");

        fixed (char* envPtr = environment) {
            debugSettings.EnableDebugging(packageFullName: packageFullName,
                                          debuggerCommandLine: debuggerCommandLine,
                                          environment: (IntPtr)envPtr)
                         .ThrowOnFailure();
        }
    }

    static char[]? MakeEnvironmentBlock(IEnumerable<KeyValuePair<string, string>>? env) {
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

    class ShowWindow {
        public const int HIDE = 0;
        public const int MINIMIZE = 2;
        public const int MAXIMIZE = 3;
    }
}