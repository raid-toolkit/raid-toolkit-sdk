using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IHost host = CreateHost(args))
            {
                await host.StartAsync();
                Thread.Sleep(600);
                await host.StopAsync();
            }

            //Application.Run(new Form1());
        }

        class Settings : IDataServiceSettings
        {
            public string InstallationPath => ".";
            public string StoragePath => @".\Data";
        }

        private static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => services
                .Configure<ProcessManagerSettings>(config =>
                {
                    config.PollIntervalMs = 100;
                    config.ProcessName = "Raid";
                })
                .AddSingleton<IDataServiceSettings>(new Settings())
                // use shared implementations:
                .AddExtensibilityServices<PackageManager>()
                .AddFeatures(HostFeatures.ProcessWatcher)
            ).Build();
    }
}
