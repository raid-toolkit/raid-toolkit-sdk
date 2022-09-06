using System;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.App.Tasks
{
    [Verb("install", HelpText = "Installs an extension")]
    internal class InstallOptions
    {
        [Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
        public string? PackagePath { get; set; }

        [Option('y', "accept")]
        public bool Accept { get; set; }

    }

    internal class InstallTask : CommandTaskBase<InstallOptions>
    {
        private InstallOptions? Options;
        private readonly IServiceProvider ServiceProvider;
        private readonly IExtensionHostController ExtensionHostController;
        private readonly IWindowManager WindowManager;
        private readonly IAppUI AppUI;

        public InstallTask(IServiceProvider serviceProvider, IExtensionHostController extensionHost, IWindowManager windowManager, IAppUI appUI)
        {
            ServiceProvider = serviceProvider;
            ExtensionHostController = extensionHost;
            WindowManager = windowManager;
            AppUI = appUI;
        }

        public override int Invoke()
        {
            if (Options == null)
                throw new NullReferenceException();

            if (Options.Accept)
                WindowManager.CanShowUI = false;

            ExtensionBundle bundleToInstall = ExtensionBundle.FromFile(Options.PackagePath);

            // bypass UI if accept was passed as an argument
            if (!Options.Accept)
            {
                bool? result = AppUI.ShowExtensionInstaller(bundleToInstall);
                if (result != true)
                {
                    return 10;
                }
            }

            ExtensionHostController.InstallPackage(bundleToInstall, activate: false);

            return 0;
        }

        public override ApplicationStartupCondition Parse(InstallOptions options)
        {
            Options = options;
            ApplicationHost.Enabled = false;
            return ApplicationStartupCondition.Services;
        }
    }
}
