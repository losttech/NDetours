using System;
using System.Linq;

using NDetours;

if (args.Length == 0) {
    Console.Error.WriteLine("Usage: detours.exe <executable> [dlls...]");
    return -1;
}

var startInfo = new ProcessDetour.StartInfo(args[0]);
foreach (string dll in args.Skip(1)) {
    startInfo.InjectDlls.Add(dll);
}

Console.Write("starting...");
using var process = ProcessDetour.Start(startInfo);
Console.WriteLine("OK");
Console.Write("running...");
process.WaitForExit();
Console.WriteLine(process.ExitCode);
return 0;
