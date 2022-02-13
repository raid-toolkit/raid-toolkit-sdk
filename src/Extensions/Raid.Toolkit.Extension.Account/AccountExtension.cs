using System;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account
{
    // TEMP: ID MUST match the DLL filename
    [ExtensionPackage("Raid.Toolkit.Extension.Account", "Account", "Extracts account data")]
    public class AccountExtension : IExtensionPackage, IDisposable
    {
        private bool IsDisposed;

        public AccountExtension(ILogger<AccountExtension> logger)
        {
            logger.LogInformation("Test");
        }

        public void OnActivate(IExtensionHost host)
        {
            //throw new System.NotImplementedException();
        }

        public void OnDeactivate(IExtensionHost host)
        {
            //throw new System.NotImplementedException();
        }

        public void OnInstall(IExtensionHost host)
        {
            //throw new System.NotImplementedException();
        }

        public void OnUninstall(IExtensionHost host)
        {
            //throw new System.NotImplementedException();
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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
        #endregion
    }
}
