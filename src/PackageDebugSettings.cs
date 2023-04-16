namespace NDetours;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using PInvoke;

[ComImport, Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
class PackageDebugSettings { }

static class PackageDebug {
    public static IPackageDebugSettings CreateSettings()
        // ReSharper disable once SuspiciousTypeConversion.Global
        => (IPackageDebugSettings)new PackageDebugSettings();
}

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "WinAPI")]
enum PackageExecutionState {
    PES_UNKNOWN,
    PES_RUNNING,
    PES_SUSPENDING,
    PES_SUSPENDED,
    PES_TERMINATED,
}

[ComImport, Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IPackageDebugSettings {
    HResult EnableDebugging([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                            [MarshalAs(UnmanagedType.LPWStr)] string? debuggerCommandLine,
                            IntPtr environment);

    HResult DisableDebugging([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
    HResult Suspend([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
    HResult Resume([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
    HResult TerminateAllProcesses([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
    HResult SetTargetSessionId(int sessionId);

    HResult EnumerateBackgroundTasks([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                     out uint taskCount, out int intPtr, [Out] string[] array);

    HResult ActivateBackgroundTask(IntPtr something);
    HResult StartServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);
    HResult StopServicing([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

    HResult StartSessionRedirection([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                    uint sessionId);

    HResult StopSessionRedirection([MarshalAs(UnmanagedType.LPWStr)] string packageFullName);

    HResult GetPackageExecutionState([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                     out PackageExecutionState packageExecutionState);

    HResult RegisterForPackageStateChanges([MarshalAs(UnmanagedType.LPWStr)] string packageFullName,
                                           IntPtr pPackageExecutionStateChangeNotification,
                                           out uint pdwCookie);

    HResult UnregisterForPackageStateChanges(uint dwCookie);
}