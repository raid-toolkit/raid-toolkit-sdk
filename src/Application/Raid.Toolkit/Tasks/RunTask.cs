using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Model;
using Raid.Toolkit.UI;

namespace Raid.Toolkit
{
    internal enum RunAction
    {
        Unknown = 0,
        Install,
        Activate,
        Run
    }
    [Verb("run", isDefault: true, HelpText = "Runs the service")]
    internal class RunOptions
    {
        [Option('s', "standalone", HelpText = "Runs in standalone mode")]
        public bool Standalone { get; set; }

        [Option('n', "no-ui", HelpText = "Runs without UI (only valid for standalone mode)")]
        public bool NoUI { get; set; }

        [Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
        public int? Wait { get; set; }

        [Option('u', "post-update")]
        public bool Update { get; set; }

        [Option('d', "debug", Hidden = true)]
        public bool Debug { get; set; }
    }

    internal class RunTask : CommandTaskBase<RunOptions>
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IModelLoader Loader;

        private RunAction RunAction = RunAction.Unknown;
        private RunOptions? Options;

        public RunTask(IServiceProvider serviceProvider, IModelLoader modelLoader)
        {
            ServiceProvider = serviceProvider;
            Loader = modelLoader;
            Loader.OnStateUpdated += Resolver_OnStateUpdated;
        }

        public override ApplicationStartupCondition Parse(RunOptions options)
        {
            Options = options;

            // Options fixup
            if (!Options.Standalone)
            {
                Options.NoUI = false;
            }

            if (!Options.Standalone &&
                (!RegistrySettings.IsInstalled ||
                    (!Options.Debug &&
                    AppHost.ExecutablePath.ToLowerInvariant() != RegistrySettings.InstalledExecutablePath.ToLowerInvariant())
                )
            )
            {
                RunAction = RunAction.Install;
                return ApplicationStartupCondition.None;
            }

            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(Options.Wait.HasValue ? TimeSpan.FromMilliseconds(Options.Wait.Value) : TimeSpan.Zero);
                if (isAnotherInstanceOpen && !Options.Standalone)
                {
                    RunAction = RunAction.Activate;
                    return ApplicationStartupCondition.None;
                }

                RunAction = RunAction.Run;
                return ApplicationStartupCondition.Services;
            }
        }

        public override Task<int> Invoke()
        {
            if (Options == null)
                return Task.FromResult(255);

            // kill existing processes for debugging
            if (Options.Debug && Debugger.IsAttached)
                KillExistingProcesses();

            switch (RunAction)
            {
                case RunAction.Install:
                    Application.Run(new InstallWindow());
                    return Task.FromResult(0);
                case RunAction.Run:
                    MainWindow mainWnd = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider, Options);
                    ServiceProvider.GetRequiredService<IExtensionHostController>().ShowExtensionUI();
                    Application.Run(mainWnd);
                    return Task.FromResult(0);
                case RunAction.Activate:
                    // TODO: Activate existing window, if desired?
                    return Task.FromResult(0);
            }

            return Task.FromResult(255);
        }

        private static void KillExistingProcesses()
        {
            var existingProcs = Process
                                .GetProcessesByName("Raid.Toolkit")
                                .Where(proc => proc.Id != Environment.ProcessId);

            foreach (var existingProc in existingProcs)
            {
                existingProc.Kill();
            }
        }

        static RebuildDialog? rebuildDialog;
        private static void Resolver_OnStateUpdated(object? sender, IModelLoader.ModelLoaderEventArgs e)
        {
            switch (e.LoadState)
            {
                case IModelLoader.LoadState.Rebuild:
                    Task.Run(() =>
                    {
                        PlariumPlayAdapter.GameInfo gameInfo = ModelLoader.GetGameInfo();
                        rebuildDialog = new RebuildDialog(gameInfo.Version);
                        rebuildDialog.ShowDialog();
                    });
                    break;
                case IModelLoader.LoadState.Ready:
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
