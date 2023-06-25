namespace NDetours;

using System.Collections.ObjectModel;

using static PInvoke.Kernel32;
using static PInvoke.User32;

partial class ProcessDetour {
    public sealed class StartInfo {
        public string Executable { get; }
        public Collection<string> InjectDlls { get; } = new();
        public string? WorkingDirectory { get; set; }
        public string? CommandLine { get; set; }
        public IDictionary<string, string>? Environment { get; set; }
        public CreateProcessFlags Flags { get; set; } = CreateProcessFlags.None;

        WindowShowStyle showWindow = WindowShowStyle.SW_SHOWDEFAULT;
        public WindowShowStyle? ShowWindow {
            get => this.showWindow == WindowShowStyle.SW_SHOWDEFAULT
                    ? null : this.showWindow;
            set {
                if (value == WindowShowStyle.SW_SHOWDEFAULT)
                    throw new ArgumentException($"{nameof(WindowShowStyle.SW_SHOWDEFAULT)} can not be used when starting new process.");

                this.showWindow = value ?? WindowShowStyle.SW_SHOWDEFAULT;
            }
        }

        public StartInfo(string executable) {
            this.Executable = executable ?? throw new ArgumentNullException(nameof(executable));
        }
    }
}