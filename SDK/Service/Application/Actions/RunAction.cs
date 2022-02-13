using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Model;
using Raid.Service.UI;

namespace Raid.Service
{
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    public class RunOptions
    {
        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }
    }

    static class RunAction
    {
        public static int Execute(RunOptions options)
        {
            if (!options.Standalone)
            {
                options.NoUI = false;

                if (!AppConfiguration.IsInstalled ||
                    (Environment.GetEnvironmentVariable("RTK_DEBUG") != "true" && AppConfiguration.ExecutablePath.ToLowerInvariant() != AppConfiguration.InstalledExecutablePath.ToLowerInvariant()))
                {
                    Application.Run(new InstallWindow());
                    return 0;
                }
            }

            if (Environment.GetEnvironmentVariable("RTK_DEBUG") == "true" && System.Diagnostics.Debugger.IsAttached)
            {
                // kill existing processes for debugging
                foreach(var existingProc in System.Diagnostics.Process.GetProcessesByName("Raid.Service").Where(proc => proc.Id != Environment.ProcessId))
                {
                    existingProc.Kill();
                }
            }

            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(options.Wait.HasValue ? TimeSpan.FromMilliseconds(options.Wait.Value) : TimeSpan.Zero);
                if (isAnotherInstanceOpen && !options.Standalone)
                {
                    return 1;
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
            return 0;
        }

        private static async Task Run(RunOptions options)
        {
            using (var resolver = new ModelAssemblyResolver())
            {
                resolver.OnStateUpdated += Resolver_OnStateUpdated;
                await resolver.Load(false);
                var host = RaidHost.CreateHost(options);
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await host.RunAsync();
                    }
                    catch (OperationCanceledException) { }
                });
                RaidHost.Services.GetRequiredService<MainService>().Run(options);
                await task;
            }
        }

        static RebuildDialog rebuildDialog;
        private static void Resolver_OnStateUpdated(object sender, ModelLoadStateEventArgs e)
        {
            switch(e.LoadState)
            {
                case ModelLoadState.Rebuild:
                    Task.Run(() =>
                    {
                        rebuildDialog = new RebuildDialog(ModelAssemblyResolver.CurrentVersion);
                        rebuildDialog.ShowDialog();
                    });
                    break;
                case ModelLoadState.Ready:
                    if (rebuildDialog != null)
                    {
                        rebuildDialog.Invoke((MethodInvoker)delegate
                        {
                            rebuildDialog.Hide();
                            rebuildDialog.Dispose();
                            rebuildDialog = null;
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
