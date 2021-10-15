using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Raid.Model;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<RegisterOptions, RunOptions>(args)
                .MapResult<RegisterOptions, RunOptions, int>(RegisterAction.Execute, RunAction.Execute, HandleErrors);
        }

        private static int HandleErrors(IEnumerable<Error> _)
        {
            return 1;
        }
    }
}