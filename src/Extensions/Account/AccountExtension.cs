using System;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extension.Account;

namespace Raid.Toolkit.Extension.Account
{
    public class AccountExtension : ExtensionPackage, IDisposable
    {
        public AccountExtension(ILogger<AccountExtension> logger)
        {
        }

        public override void OnActivate(IExtensionHost host)
        {
#pragma warning disable CS0612 // Type or member is obsolete
            Disposables.Add(host.RegisterDataProvider<StaticAcademyProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticArenaProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticArtifactProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticHeroTypeProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticLocalizationProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticSkillProvider>());
            Disposables.Add(host.RegisterDataProvider<StaticStageProvider>());
            Disposables.Add(host.RegisterMessageScopeHandler(host.CreateInstance<StaticDataApi>(host)));

            Disposables.Add(host.RegisterDataProvider<AcademyProvider>());
            Disposables.Add(host.RegisterDataProvider<AccountInfoProvider>());
            Disposables.Add(host.RegisterDataProvider<ArenaProvider>());
            Disposables.Add(host.RegisterDataProvider<ArtifactsProvider>());
            Disposables.Add(host.RegisterDataProvider<HeroesProvider>());
            Disposables.Add(host.RegisterDataProvider<ResourcesProvider>());
            Disposables.Add(host.RegisterMessageScopeHandler<AccountApi>());

            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<StaticTypesExtension>(host, true)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ArtifactExtension>(host, false)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<HeroesExtension>(host, false)));
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }
}
