namespace NDetours;

using System.Collections.ObjectModel;

using static PInvoke.Kernel32;

partial class ProcessDetour {
    public sealed class StartInfo {
        public string Executable { get; }
        public Collection<string> InjectDlls { get; } = new();
        public string? WorkingDirectory { get; set; }
        public string? CommandLine { get; set; }
        public IDictionary<string, string>? Environment { get; set; }
        public CreateProcessFlags Flags { get; set; } = CreateProcessFlags.None;

        public StartInfo(string executable) {
            this.Executable = executable ?? throw new ArgumentNullException(nameof(executable));
        }
    }
}