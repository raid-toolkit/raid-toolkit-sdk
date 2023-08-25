using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility.Host;

public interface IAccountManager
{
    IEnumerable<IAccount> Accounts { get; }
    bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account);
    string ExportAccountData(string accountId);
    void ImportAccountData(string accountData);
    void RegisterAccountExtension<T>(ExtensionManifest manifest, T factory) where T : IAccountExtensionFactory;
    void UnregisterAccountExtension<T>(ExtensionManifest manifest, T factory) where T : IAccountExtensionFactory;
}
