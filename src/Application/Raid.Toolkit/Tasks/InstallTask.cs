using System;
using System.Windows.Forms;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.UI;
using InstallExtensionDialog = Raid.Toolkit.UI.Windows.InstallExtensionDialog;

namespace Raid.Toolkit
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

        public InstallTask(IServiceProvider serviceProvider, IExtensionHostController extensionHost, IWindowManager windowManager)
        {
            ServiceProvider = serviceProvider;
            ExtensionHostController = extensionHost;
            WindowManager = windowManager;
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
                InstallExtensionDialog dlg = ActivatorUtilities.CreateInstance<InstallExtensionDialog>(ServiceProvider, bundleToInstall);
                bool? result = dlg.ShowDialog();
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
