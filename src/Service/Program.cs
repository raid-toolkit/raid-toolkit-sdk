using System.Collections.Generic;
using CommandLine;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<RegisterOptions, OpenOptions, RunOptions>(args)
                .MapResult<RegisterOptions, OpenOptions, RunOptions, int>(RegisterAction.Execute, OpenAction.Execute, RunAction.Execute, HandleErrors);
        }

        private static int HandleErrors(IEnumerable<Error> _)
        {
            return 1;
        }
    }
}