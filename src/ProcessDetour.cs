namespace NDetours;

using System.Diagnostics;
using System.Text;

using PInvoke;

public static class ProcessDetour {
    public static unsafe Process Start(string exe,
                                       IReadOnlyDictionary<string, string> env,
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
            char[] envBlock = MakeEnvironmentBlock(env);

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

    static char[] MakeEnvironmentBlock(IReadOnlyDictionary<string, string> env) {
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