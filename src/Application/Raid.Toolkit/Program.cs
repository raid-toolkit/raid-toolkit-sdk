using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using System;
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
                await host.Services.GetRequiredService<ApplicationStartupTask>().Execute(args);
                await host.StopAsync();
            }
        }

        class Settings : IDataServiceSettings
        {
            public string InstallationPath => ".";
            public string StoragePath => @".\Data";
        }

        private static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => services
                // app dependencies
                .Configure<ProcessManagerSettings>(config =>
                {
                    config.PollIntervalMs = 100;
                    config.ProcessName = "Raid";
                })
                .AddSingleton<IDataServiceSettings>(new Settings())
                .AddSingleton<ApplicationStartupTask>()
                // shared dependencies
                .AddExtensibilityServices<PackageManager>()
                .AddFeatures(HostFeatures.ProcessWatcher)
            ).Build();
    }
}
