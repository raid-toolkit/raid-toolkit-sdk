using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            Type t = typeof(Newtonsoft.Json.JsonSerializer);
            return Parser.Default.ParseArguments<RegisterOptions, OpenOptions, RunOptions>(args)
                .MapResult<RegisterOptions, OpenOptions, RunOptions, Task<int>>(RegisterAction.Execute, OpenAction.Execute, RunAction.Execute, HandleErrors);
        }

        private static Task<int> HandleErrors(IEnumerable<Error> _)
        {
            return Task.FromResult(1);
        }
    }
}