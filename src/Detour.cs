namespace NDetours;

using System.Runtime.InteropServices;

using static PInvoke.Kernel32;

public static class Detour {
    const string DLL = "detours.dll";

    [DllImport(DLL, SetLastError = true, CharSet = CharSet.Auto)]
    public static extern unsafe bool CreateProcessWithDlls(string exe, string commandLine,
                                                           SECURITY_ATTRIBUTES* processAttributes,
                                                           SECURITY_ATTRIBUTES* threadAttributes,
                                                           bool inheritHandles,
                                                           CreateProcessFlags flags,
                                                           IntPtr lpEnvironment,
                                                           string currentDirectory,
                                                           ref STARTUPINFO startupInfo,
                                                           out PROCESS_INFORMATION processInfo,
                                                           Int32 dllsCount,
                                                           char*[] dlls,
                                                           IntPtr createProcess
    );
}