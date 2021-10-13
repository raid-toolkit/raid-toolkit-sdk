using System;
using System.Collections.Concurrent;
using System.Threading;
using Raid.Model;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        private static ModelService modelService;
        static int Main(string[] args)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Console.CancelKeyPress += delegate
            {
                mre.Set();
            };
            using (new ModelAssemblyResolver())
            {
                Start();

                mre.WaitOne();

                modelService.Stop();
            }
            return 0;
        }

        private static void Start()
        {
            ProcessWatcher processWatcher = new("Raid");
            processWatcher.ProcessFound += ProcessFound;
            TaskExtensions.RunAfter(2000, UpdateAccounts);

            modelService = new();
            modelService.Start();
        }

        private static void UpdateAccounts()
        {
            foreach (var instance in RaidInstance.Instances)
            {
                try
                {
                    instance.Update();
                }
                catch (Exception)
                {
                    // TODO: Logging
                }
            }
            TaskExtensions.RunAfter(10000, UpdateAccounts);
        }

        private static void ProcessFound(object sender, ProcessWatcher.ProcessWatcherEventArgs e)
        {
            RaidInstance instance = new(e.Process);
        }
    }
}