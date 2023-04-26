namespace NDetours;

using ManyConsole.CommandLineUtils;

public static class Launcher {
    static int Main(string[] args) {
        Console.WriteLine(string.Join(Environment.NewLine, args));
        return ConsoleCommandDispatcher
            .DispatchCommand(
                new ConsoleCommand[] { new InjectCommand() },
                args,
                consoleOut: TextWriter.Null);
    }

    public static string GetExePath()
        => Path.ChangeExtension(typeof(Launcher).Assembly.Location, ".exe");
}