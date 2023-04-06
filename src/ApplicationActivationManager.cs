namespace NDetours;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using PInvoke;

enum ActivateOptions {
    None = 0x00000000,

    /// <summary>
    /// The application is being activated for design mode, and thus will not be able to
    /// to create an immersive window. Window creation must be done by design tools which
    /// load the necessary components by communicating with a designer-specified service on
    /// the site chain established on the activation manager.  The splash screen normally
    /// shown when an application is activated will also not appear.  Most activations
    /// will not use this flag.
    /// </summary>
    DesignMode = 0x00000001,

    /// <summary>Do not show an error dialog if the app fails to activate.</summary>
    NoErrorUI = 0x00000002,

    /// <summary>Do not show the splash screen when activating the app.</summary>
    NoSplashScreen = 0x00000004,
    Prelaunch = 0x2000000,
}

[ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IApplicationActivationManager {
    /// <summary>Activates the specified immersive application for the "Launch" contract,
    /// passing the provided arguments string into the application.
    /// Callers can obtain the process Id of the application instance fulfilling this contract.
    /// </summary>
    HResult ActivateApplication([In] String appUserModelId, [In] String? arguments,
                                [In] ActivateOptions options, [Out] out Int32 processId);

    HResult ActivateForFile([In] String appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray,
                            [In] String verb, [Out] out Int32 processId);

    HResult ActivateForProtocol([In] String appUserModelId,
                                [In] IntPtr /* IShellItemArray* */itemArray,
                                [Out] out Int32 processId);
}

[ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
class ApplicationActivationManager: IApplicationActivationManager {
    [MethodImpl(MethodImplOptions.InternalCall,
                MethodCodeType = MethodCodeType.Runtime) /*, PreserveSig*/]
    public extern HResult ActivateApplication([In] String appUserModelId, [In] String? arguments,
                                              [In] ActivateOptions options,
                                              [Out] out Int32 processId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern HResult ActivateForFile([In] String appUserModelId,
                                          [In] IntPtr /*IShellItemArray* */ itemArray,
                                          [In] String verb, [Out] out Int32 processId);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern HResult ActivateForProtocol([In] String appUserModelId,
                                              [In] IntPtr /* IShellItemArray* */itemArray,
                                              [Out] out Int32 processId);
}