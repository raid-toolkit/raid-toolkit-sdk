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
            try
            {
                return Parser.Default.ParseArguments<OpenOptions, RunOptions>(args)
                    .MapResult<OpenOptions, RunOptions, Task<int>>(OpenAction.Execute, RunAction.Execute, HandleErrors);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return Task.FromResult(3);
            }
        }

        private static Task<int> HandleErrors(IEnumerable<Error> _)
        {
            return Task.FromResult(1);
        }
    }
}