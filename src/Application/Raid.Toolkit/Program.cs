using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using System;
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

        class PackageFactory : IPackageInstanceFactory
        {
            private readonly IServiceProvider ServiceProvider;

            public PackageFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

            public IExtensionPackage CreateInstance(Type type)
            {
                return (IExtensionPackage)ActivatorUtilities.CreateInstance(ServiceProvider, type);
            }
        }

        class ProgramHost : IDisposable
        {
            private readonly ExtensionHost Host;
            private bool IsDisposed;

            public ProgramHost(ExtensionHost host) => (Host) = (host);

            public void Run()
            {
                Host.LoadExtensions();
                Host.ActivateExtensions();
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
                .AddSingleton<ExtensionHost>()
                .AddSingleton<IPackageInstanceFactory, PackageFactory>()
                .AddSingleton<IPackageLoader, SandboxedPackageLoader>()
                .AddSingleton<IPackageManager, PackageManager>()
            ).Build();
    }
}
