using System;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account
{
    public class AccountExtension : ExtensionPackage, IDisposable
    {
        public AccountExtension(ILogger<AccountExtension> logger)
        {
        }

        public override void OnActivate(IExtensionHost host)
        {
            Disposables.Add(host.RegisterDataProvider<StaticAcademyProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticArenaProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticArtifactProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticHeroTypeProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticLocalizationProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticSkillProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticStageProvider>());
            Disposables.Add(host.RegisterMessageScopeHandler<StaticDataApi>());

            Disposables.Add(host.RegisterDataProvider<AcademyProvider>());
            Disposables.Add(host.RegisterDataProvider<AccountInfoProvider>());
            Disposables.Add(host.RegisterDataProvider<ArenaProvider>());
            Disposables.Add(host.RegisterDataProvider<ArtifactsProvider>());
            Disposables.Add(host.RegisterDataProvider<HeroesProvider>());
            Disposables.Add(host.RegisterDataProvider<ResourcesProvider>());
            Disposables.Add(host.RegisterMessageScopeHandler<AccountApi>());
        }
    }
}
