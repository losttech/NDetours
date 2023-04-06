namespace NDetours;

using System.Runtime.InteropServices;

using static PInvoke.Kernel32;

public static class Detour {
    const string DLL = "detours.dll";

    [DllImport(DLL, SetLastError = true, CharSet = CharSet.Unicode,
               EntryPoint = "DetourCreateProcessWithDllsW")]
    public static extern unsafe bool CreateProcessWithDlls(string exe, string? commandLine,
                                                           SECURITY_ATTRIBUTES* processAttributes,
                                                           SECURITY_ATTRIBUTES* threadAttributes,
                                                           bool inheritHandles,
                                                           CreateProcessFlags flags,
                                                           char[]? lpEnvironment,
                                                           string? currentDirectory,
                                                           ref STARTUPINFO startupInfo,
                                                           out PROCESS_INFORMATION processInfo,
                                                           Int32 dllsCount,
                                                           byte*[] dlls,
                                                           IntPtr createProcess
    );

    [DllImport(DLL, SetLastError = true,
               EntryPoint = "DetourUpdateProcessWithDll")]
    public static extern bool UpdateProcessWithDll(
        SafeObjectHandle process,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 2)]
        string[] dlls,
        Int32 dllsCount);

    [DllImport(DLL, SetLastError = true,
               EntryPoint = "DetourUpdateProcessWithDllEx")]
    public static extern bool UpdateProcessWithDllEx(
        SafeObjectHandle process,
        SafeLibraryHandle module,
        bool is32BitProcess,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)]
        string[] dlls,
        Int32 dllsCount);
}