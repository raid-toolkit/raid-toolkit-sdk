using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility.Host;

public class AccountManagerEventArgs
{
	public IAccount Account { get; }
	public AccountManagerEventArgs(IAccount account)
	{
		Account = account;
	}
}

public interface IAccountManager
{
	event EventHandler<AccountManagerEventArgs> OnAdded;
	event EventHandler<AccountManagerEventArgs> OnRemoved;

	IEnumerable<IAccount> Accounts { get; }
	bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account);
	string ExportAccountData(string accountId);
	void ImportAccountData(string accountData);
	void RegisterAccountExtension<T>(ExtensionManifest manifest, T factory) where T : IAccountExtensionFactory;
	void UnregisterAccountExtension<T>(ExtensionManifest manifest, T factory) where T : IAccountExtensionFactory;
}
