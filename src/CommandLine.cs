namespace NDetours;

#if WINDOWS10_0_17763_0_OR_GREATER
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using ManyConsole.CommandLineUtils;

static class CommandLine {
    static int Main(string[] args) {
        if (args.Contains("--debug")) {
            Thread.Sleep(30_000);
        }

        Debug.WriteLine(string.Join(Environment.NewLine, args));

        return ConsoleCommandDispatcher.DispatchCommand(
            ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(CommandLine)),
            args,
            new DebugTextWriter(Console.Out));
    }

    public static string ExePath {
        get {
            string exeOrDll = Assembly.GetExecutingAssembly().Location;

            string exe = ".dll".Equals(Path.GetExtension(exeOrDll),
                                       StringComparison.InvariantCultureIgnoreCase)
                ? Path.ChangeExtension(exeOrDll, ".exe")
                : exeOrDll;

            if (!File.Exists(exe))
                throw new FileNotFoundException();

            return exe;
        }
    }
}
#endif