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
            Disposables.Add(host.RegisterMessageScopeHandler(host.CreateInstance<StaticDataApi>(host)));
            Disposables.Add(host.RegisterMessageScopeHandler(host.CreateInstance<AccountApi>(host)));

            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<StaticTypesExtension>(host, true)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ArtifactExtension>(host, false)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<HeroesExtension>(host, false)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ArenaExtension>(host, false)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<AcademyExtension>(host, false)));
            Disposables.Add(host.RegisterAccountExtension(new AccountDataExtensionFactory<ResourcesExtension>(host, false)));
#pragma warning restore CS0612 // Type or member is obsolete
        }
    }
}
