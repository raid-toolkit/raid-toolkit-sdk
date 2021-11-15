using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<OpenOptions, RunOptions>(args)
                    .MapResult<OpenOptions, RunOptions, int>(OpenAction.Execute, RunAction.Execute, HandleErrors);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 3;
            }
        }

        private static int HandleErrors(IEnumerable<Error> _)
        {
            return 1;
        }
    }
}