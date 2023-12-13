namespace NDetours;

using System.Text.Json;
using static NDetours.ProcessDetour;

public class AsUser {
    [Fact]
    public void StartInfoRoundtrip() {
        var original = new StartInfo(@"C:\test.exe") {
            InjectDlls = { "a.dll", "b.dll" },
            WorkingDirectory = @"C:\",
        };
        string json = JsonSerializer.Serialize(original);
        var startInfo = JsonSerializer.Deserialize<StartInfo>(json)!;
        Assert.Equal(original.Executable, startInfo.Executable);
        Assert.Equal(original.WorkingDirectory, startInfo.WorkingDirectory);
        Assert.Equal(original.InjectDlls, startInfo.InjectDlls);
    }
}