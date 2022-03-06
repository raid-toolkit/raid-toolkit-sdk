using System;
using System.Threading.Tasks;
using Raid.Toolkit.Extensibility.HostInterfaces;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ApplicationHost : IApplicationHost, IDisposable
    {
        private readonly IExtensionHostController ExtensionHost;
        private readonly IEntryPoint EntryPoint;
        private bool IsDisposed;

        public ApplicationHost(IExtensionHostController extensionHost, IEntryPoint entryPoint)
        {
            ExtensionHost = extensionHost;
            EntryPoint = entryPoint;
        }

        public async Task<int> Run(IRunArguments args)
        {
            await ExtensionHost.LoadExtensions();
            ExtensionHost.ActivateExtensions();
            EntryPoint.Run(args);
            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                IsDisposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApplicationHost()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}