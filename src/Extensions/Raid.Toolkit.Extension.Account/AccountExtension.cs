using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account
{
    // TEMP: ID MUST match the DLL filename
    [ExtensionPackage("Raid.Toolkit.Extension.Account", "Account", "Extracts account data")]
    public class AccountExtension : IExtensionPackage, IRequireCodegen, IDisposable
    {
        private static readonly CodegenTypeFilter kTypeFilter = new(new[] {
            new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.Artifacts\.ExternalArtifactsStorage$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.StaticData\.ClientStaticDataManager$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^SharedModel\.Meta\.Artifacts\.ArtifactStorage\.ArtifactStorageResolver$", RegexOptions.Singleline | RegexOptions.Compiled)
        });

        private DisposableCollection Handles = new();
        private bool IsDisposed;

        public CodegenTypeFilter TypeFilter => kTypeFilter;

        public AccountExtension(ILogger<AccountExtension> logger)
        {
            logger.LogInformation("Test");
        }

        public void OnActivate(IExtensionHost host)
        {
            Handles.Add(host.RegisterDataProvider<StaticAcademyProvider>());
            Handles.Add(host.RegisterDataProvider<StaticArenaProvider>());
            Handles.Add(host.RegisterDataProvider<StaticArtifactProvider>());
            Handles.Add(host.RegisterDataProvider<StaticHeroTypeProvider>());
            Handles.Add(host.RegisterDataProvider<StaticLocalizationProvider>());
            Handles.Add(host.RegisterDataProvider<StaticSkillProvider>());
            Handles.Add(host.RegisterDataProvider<StaticStageProvider>());

            Handles.Add(host.RegisterDataProvider<AcademyProvider>());
            Handles.Add(host.RegisterDataProvider<AccountInfoProvider>());
            Handles.Add(host.RegisterDataProvider<ArenaProvider>());
            Handles.Add(host.RegisterDataProvider<ArtifactsProvider>());
            Handles.Add(host.RegisterDataProvider<HeroesProvider>());
            Handles.Add(host.RegisterDataProvider<ResourcesProvider>());
            Handles.Add(host.RegisterMessageScopeHandler<AccountApi>());
        }

        public void OnDeactivate(IExtensionHost host)
        {
            Handles.Dispose();
            Handles = new();
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
