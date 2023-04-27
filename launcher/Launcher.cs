namespace NDetours;

using ManyConsole.CommandLineUtils;

public static class Launcher {
    static int Main(string[] args) {
        Console.WriteLine(string.Join(Environment.NewLine, args));
        if (args.Contains("--debug"))
            Thread.Sleep(TimeSpan.FromSeconds(30));

        try {
            return ConsoleCommandDispatcher
                .DispatchCommand(
                    new ConsoleCommand[] { new InjectCommand() },
                    args,
                    consoleOut: TextWriter.Null);
        } catch (Exception ex) {
            Console.Error.WriteLine(ex.ToString());
            if (args.Contains("--debug")) {
                Thread.Sleep(TimeSpan.FromSeconds(30));
            }
            throw;
        }
    }

    public static string GetExePath()
        => Path.ChangeExtension(typeof(Launcher).Assembly.Location, ".exe");
}