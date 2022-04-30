using System;
using System.Diagnostics;
using System.IO;
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

        [Option('p', "debug-package", Hidden = true)]
        public string? DebugPackage { get; set; }

        [Option('m', "no-default-packages", Hidden = true)]
        public bool NoDefaultPackages { get; set; }

        [Option('i', "--interop-dir", Hidden = true)]
        public string? InteropDirectory { get; set; }
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
            PackageManager.DebugPackage = options.DebugPackage;
            PackageManager.NoDefaultPackages = options.NoDefaultPackages;
            if (!string.IsNullOrEmpty(PackageManager.DebugPackage))
            {
                Options.Debug = true;
                string debugInteropDirectory = Path.Combine(PackageManager.DebugPackage, @"temp~interop");
                Loader.OutputDirectory = debugInteropDirectory;
            }
            if (!string.IsNullOrEmpty(options.InteropDirectory))
            {
                Loader.OutputDirectory = options.InteropDirectory;
            }

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

            using var mutex = new Mutex(false, "RaidToolkit Singleton");
            bool isAnotherInstanceOpen = !mutex.WaitOne(Options.Wait.HasValue ? TimeSpan.FromMilliseconds(Options.Wait.Value) : TimeSpan.Zero);
            if (isAnotherInstanceOpen && !Options.Standalone)
            {
                RunAction = RunAction.Activate;
                return ApplicationStartupCondition.None;
            }

            RunAction = RunAction.Run;
            return ApplicationStartupCondition.Services;
        }

        public override int Invoke()
        {
            if (Options == null)
                return 255;

            // kill existing processes for debugging
            if (Options.Debug && !Options.Standalone && Debugger.IsAttached)
                KillExistingProcesses();

            switch (RunAction)
            {
                case RunAction.Install:
                    Application.Run(new InstallWindow());
                    return 0;
                case RunAction.Run:
                    using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
                    {
                        bool releaseMutex = false;
                        if (!Options.Standalone)
                        {
                            bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                            if (isAnotherInstanceOpen && Options.Standalone)
                            {
                                return 0;
                            }
                            releaseMutex = true;
                        }
                        try
                        {
                            if (!Options.Standalone)
                            {
                                AppHost.EnsureFileAssociations();
                            }
                            MainWindow mainWnd = ActivatorUtilities.CreateInstance<MainWindow>(ServiceProvider, Options);
                            ServiceProvider.GetRequiredService<IExtensionHostController>().ShowExtensionUI();
                            Application.Run(mainWnd);
                        }
                        finally
                        {
                            if (releaseMutex)
                                mutex.ReleaseMutex();
                        }
                    }
                    return 0;
                case RunAction.Activate:
                    // TODO: Activate existing window, if desired?
                    return 0;
                case RunAction.Unknown:
                    break;
                default:
                    break;
            }

            return 255;
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

        private static RebuildDialog? rebuildDialog;
        private static void Resolver_OnStateUpdated(object? sender, IModelLoader.ModelLoaderEventArgs e)
        {
            switch (e.LoadState)
            {
                case IModelLoader.LoadState.Rebuild:
                    _ = Task.Run(() =>
                    {
                        PlariumPlayAdapter.GameInfo gameInfo = ModelLoader.GetGameInfo();
                        rebuildDialog = new RebuildDialog(gameInfo.Version);
                        _ = rebuildDialog.ShowDialog();
                    });
                    break;
                case IModelLoader.LoadState.Ready:
                    if (rebuildDialog != null)
                    {
                        _ = rebuildDialog.Invoke((MethodInvoker)delegate
                        {
                            rebuildDialog.Hide();
                            rebuildDialog.Dispose();
                            rebuildDialog = null;
                        });
                    }
                    break;
                case IModelLoader.LoadState.Initialize:
                    break;
                case IModelLoader.LoadState.Loaded:
                    break;
                case IModelLoader.LoadState.Error:
                    break;
                default:
                    break;
            }
        }
    }
}
