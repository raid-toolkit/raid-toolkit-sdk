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
            using (var scope = host.Services.CreateScope())
            using (var progHost = scope.ServiceProvider.GetRequiredService<IApplicationHost>())
            {
                host.Start();
                await progHost.Run(new RunArguments());
                await host.StopAsync();
            }

            //Application.Run(new Form1());
        }

        class RunArguments : IRunArguments
        {
            public bool Standalone { get; set; }
            public bool NoUI { get; set; }
            public int? Wait { get; set; }
            public bool Update { get; set; }
        }

        class EntryPoint : IEntryPoint
        {
            public void Run(IRunArguments arguments)
            {
                Thread.Sleep(600);
            }

            public void Restart(IRunArguments arguments)
            {
                // throw new NotImplementedException();
            }

            public void Exit()
            {
                // throw new NotImplementedException();
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
                .AddExtensibilityServices<PackageManager>()
                .AddSingleton<IDataServiceSettings>(new Settings())
                .AddSingleton<IEntryPoint, EntryPoint>()
                .Configure<ProcessManagerSettings>(config =>
                {
                    config.PollIntervalMs = 100;
                    config.ProcessName = "Raid";
                })
            ).Build();
    }
}
