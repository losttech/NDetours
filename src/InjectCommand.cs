namespace NDetours;

#if WINDOWS10_0_17763_0_OR_GREATER
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using ManyConsole.CommandLineUtils;

using PInvoke;

using static PInvoke.Kernel32;

public class InjectCommand: ConsoleCommand {
    public override int Run(string[] remainingArguments) {
        if (this.EnableDebugging && !Debugger.IsAttached) {
            Thread.Sleep(TimeSpan.FromSeconds(30));
        }

        this.CheckRequiredArguments();

        uint access = ProcessAccess.PROCESS_VM_OPERATION
                    | ProcessAccess.PROCESS_VM_READ
                    | ProcessAccess.PROCESS_VM_WRITE;
        if (this.Resume)
            access |= ProcessAccess.PROCESS_SUSPEND_RESUME;

        using var process = OpenProcess(access, bInheritHandle: false, dwProcessId: this.ProcessID);
        if (process.IsInvalid)
            throw new Win32Exception();

        Debug.WriteLine("opened process");

        try {
            if (!Detour.UpdateProcessWithDll(process, this.DLLs.ToArray(), this.DLLs.Count))
                throw new Win32Exception();

            Debug.WriteLine("injected DLLs");
        } finally {
            if (this.Resume) {
                NtResumeProcess(process);
                Debug.WriteLine("resumed process");
            }
        }

        return 0;
    }

    public int ProcessID { get; set; }
    public int ThreadID { get; set; }
    public List<string> DLLs { get; set; } = new();
    public bool Resume { get; set; }
    public bool EnableDebugging { get; set; }

    [DllImport("ntdll.dll", PreserveSig = false, SetLastError = true)]
    static extern void NtResumeProcess(SafeObjectHandle processHandle);

    public InjectCommand() {
        this.IsCommand("inject");
        this.HasRequiredOption("p|pid=", "The process to inject into",
                               (int pid) => this.ProcessID = pid);
        this.HasOption("-tid=", "The main thread", (int tid) => this.ThreadID = tid);
        this.HasOption("-dll=", "Add a DLL to inject", this.DLLs.Add);
        this.HasOption("resume:", "Resume the process after injecting",
                       s => this.Resume = s is null || s == "true");
        this.HasOption("debug:", "Wait for debugger to attach",
                       s => this.EnableDebugging = s is null || s == "true");
    }
}
#endif