using Il2CppToolkit.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Raid.Toolkit
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IHost host = CreateHost(args))
            using (var scope = host.Services.CreateScope())
            using (var progHost = scope.ServiceProvider.GetRequiredService<ProgramHost>())
            {
                progHost.Run();
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

            public void Run()
            {
                Host.LoadExtensions();
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

        private static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices(services => services
                .AddSingleton<ProgramHost>()
                .AddExtensibilityServices<PackageManager>()
            ).Build();
    }
}
