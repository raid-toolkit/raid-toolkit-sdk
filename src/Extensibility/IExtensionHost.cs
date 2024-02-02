using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility;

public interface IExtensionHost
{
	T CreateInstance<T>(params object[] args);

	IEnumerable<IAccount> GetAccounts();
	bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account);

	IExtensionStorage GetStorage(bool enableCache);
	IExtensionStorage GetStorage(IAccount account, bool enableCache);

	IDisposable RegisterMessageScopeHandler<T>(T handler) where T : IMessageScopeHandler;
	IDisposable RegisterBackgroundService<T>(T service) where T : IBackgroundService;
	IDisposable RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory;

	IDisposable RegisterMenuEntry(IMenuEntry entry);

	bool CanShowUI { get; }
}
