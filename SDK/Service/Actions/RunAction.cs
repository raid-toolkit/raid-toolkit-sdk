using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Raid.Model;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    public class RunOptions
    {
        [Option('s', "--standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "--no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }
    }
    static class RunAction
    {
        public static Task<int> Execute(RunOptions options)
        {
            if (!options.Standalone)
            {
                options.NoUI = false;
            }
            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                if (isAnotherInstanceOpen && !options.Standalone)
                {
                    return Task.FromResult(1);
                }

                try
                {
                    Run(options).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            Application.ExitThread();
            return Task.FromResult(0);
        }

        private static Task Run(RunOptions options)
        {
            using (new ModelAssemblyResolver())
            {
                var host = RaidHost.CreateHost();
                var task = Task.Run(async () =>
                {
                    await host.RunAsync();
                });
                RaidHost.Services.GetRequiredService<MainService>().Run(options);
                return task;
            }
        }
    }
}