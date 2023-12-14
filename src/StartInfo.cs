namespace NDetours;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

using static PInvoke.Kernel32;

partial class ProcessDetour {
    public sealed class StartInfo {
        public string Executable { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
#if NETSTANDARD2_0
        public Collection<string> InjectDlls { get; set; } = new();
#else
        public Collection<string> InjectDlls { get; init; } = new();
#endif
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? WorkingDirectory { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? CommandLine { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, string>? Environment { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public CreateProcessFlags Flags { get; set; } = CreateProcessFlags.None;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ProcessWindowStyle WindowStyle { get; set; }

        public StartInfo(string executable) {
            this.Executable = executable ?? throw new ArgumentNullException(nameof(executable));
        }
    }
}