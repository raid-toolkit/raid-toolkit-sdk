using Microsoft.Extensions.DependencyInjection;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Diagnostics;
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
            using (var progHost = scope.ServiceProvider.GetRequiredService<ProgramHost>())
            {
                await progHost.Run();
            }

            //Application.Run(new Form1());
        }

        class ProgramHost : IDisposable
        {
            private readonly ExtensionHost Host;
            IContextDataManager DataManager;
            private bool IsDisposed;

            public ProgramHost(ExtensionHost host, IContextDataManager dataManager)
                => (Host, DataManager) = (host, dataManager);

            public async Task Run()
            {
                await Host.LoadExtensions();
                Host.ActivateExtensions();
                var proc = Process.GetProcessesByName("Raid")[0];
                Il2CsRuntimeContext runtime = new(proc);
                DataManager.Update(runtime, StaticDataContext.Default);
                DataManager.Update(runtime, new AccountDataContext("foobar"));
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!IsDisposed)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects)
                    }
                    IsDisposed = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
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
                .AddSingleton<ProgramHost>()
                .AddSingleton<IDataServiceSettings>(new Settings())
                .AddExtensibilityServices<PackageManager>()
            ).Build();
    }
}
