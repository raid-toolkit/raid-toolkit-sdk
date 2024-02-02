using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Client.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host;

public class AccountManager : PollingBackgroundService, IAccountManagerInternals
{
	private readonly static TimeSpan kPollInterval = new(0, 0, 0, 0, 50);
	private protected override TimeSpan PollInterval => kPollInterval;

	private readonly object _syncRoot = new();
	private readonly Dictionary<string, AccountInstance> AccountMap = new();
	private readonly List<ExtensionOwnedValue<IAccountExtensionFactory>> Factories = new();

	private readonly IServiceProvider ServiceProvider;
	private readonly PersistedDataStorage Storage;
	private readonly IGameInstanceManager GameInstanceManager;

	public IEnumerable<IAccount> Accounts => AccountMap.Values;

	public event EventHandler<AccountManagerEventArgs>? OnAdded;
	public event EventHandler<AccountManagerEventArgs>? OnRemoved;

	public AccountManager(
		IServiceProvider serviceProvider,
		ILogger<AccountManager> logger,
		IGameInstanceManager gameInstanceManager,
		PersistedDataStorage storage)
		: base(logger)
	{
		ServiceProvider = serviceProvider;
		GameInstanceManager = gameInstanceManager;
		Storage = storage;

		GameInstanceManager.OnAdded += GameInstanceManager_OnAdded;
		GameInstanceManager.OnRemoved += GameInstanceManager_OnRemoved;
		InitializeFromStorage();
	}

	public bool TryGetAccount(string accountId, [NotNullWhen(true)] out AccountInstance? account)
	{
		lock (_syncRoot)
			return AccountMap.TryGetValue(accountId, out account);
	}

	public bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account)
	{
		AccountInstance? result;
		bool found = false;
		lock (_syncRoot)
		{
			found = AccountMap.TryGetValue(accountId, out result);
		}
		account = result;
		return found;
	}

	private void GameInstanceManager_OnAdded(object? sender, GameInstanceAddedEventArgs e)
	{
		if (TryGetAccount(e.Instance.Id, out IAccount? value) && value is AccountInstance account)
		{
			account.Connect(e.Instance);
		}
		else
		{
			var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(e.Instance.Runtime);
			var userWrapper = appModel._userWrapper;
			var accountData = userWrapper.Account.AccountData;
			var gameSettings = userWrapper.UserGameSettings.GameSettings;
			var socialWrapper = userWrapper.Social.SocialData;
			var globalId = socialWrapper.PlariumGlobalId;
			var socialId = socialWrapper.SocialId;
			Storage.Write(new AccountDataContext(e.Instance.Id), "info.json", new AccountBase(
				string.Join("_", globalId, socialId).Sha256(),
				gameSettings.Avatar.ToString(),
				((int)gameSettings.Avatar).ToString(),
				gameSettings.Name,
				accountData.Level, (int)Math.Round(accountData.TotalPower, 0)));
			Storage.Flush();
			LoadAccount(e.Instance.Id);
		}
	}

	private void GameInstanceManager_OnRemoved(object? sender, GameInstanceRemovedEventArgs e)
	{
		if (TryGetAccount(e.Id, out IAccount? value) && value is AccountInstance account)
		{
			OnRemoved?.Invoke(this, new(account));
			account.Disconnect();
		}
	}

	private void InitializeFromStorage()
	{
		foreach (string accountId in Storage.GetKeys(new AccountDirectoryContext()))
		{
			try
			{
				LoadAccount(accountId);
			}
			catch { }
		}
	}

	private AccountInstance LoadAccount(string accountId)
	{
		AccountInstance account = ActivatorUtilities.CreateInstance<AccountInstance>(ServiceProvider, accountId);

		lock (_syncRoot)
		{
			foreach (ExtensionOwnedValue<IAccountExtensionFactory> factory in Factories)
			{
				account.AddExtension(factory.Manifest, factory.Value);
			}
			account.LoadFromStorage();
			AccountMap.TryAdd(accountId, account);
			OnAdded?.Invoke(this, new(account));
			if (GameInstanceManager.TryGetById(accountId, out ILoadedGameInstance? instance))
				account.Connect(instance);
		}
		return account;
	}

	protected override Task ExecuteOnceAsync(CancellationToken token)
	{
		AccountInstance[] accounts;
		lock (_syncRoot)
		{
			accounts = AccountMap.Values.ToArray();
		}
		foreach (AccountInstance account in accounts)
		{
			if (!AccountMap.ContainsValue(account))
				continue;

			Stopwatch swScoped = Stopwatch.StartNew();
			Logger.LogDebug("Background processing for account {account}", account.Id);
			try
			{
				account.Tick();
			}
			catch (Exception e)
			{
				Logger.LogError(e, "Failed background processing for account {account}", account.Id);
			}
			Logger.LogDebug("Background processing for account {account} completed in {ms}ms", account.Id, swScoped.ElapsedMilliseconds);
		}
		return Task.CompletedTask;
	}

	public void RegisterAccountExtension<T>(PackageManifest manifest, T factory) where T : IAccountExtensionFactory
	{
		lock (_syncRoot)
		{
			Factories.Add(new(manifest, factory));
			foreach (AccountInstance account in AccountMap.Values)
			{
				account.AddExtension(manifest, factory);
			}
		}
	}

	public void UnregisterAccountExtension<T>(PackageManifest manifest, T factory) where T : IAccountExtensionFactory
	{
		lock (_syncRoot)
		{
			Factories.RemoveAll(factory => factory.Value == factory);

			foreach (AccountInstance account in AccountMap.Values)
			{
				account.RemoveExtension(factory);
			}
		}
	}

	public string ExportAccountData(string accountId)
	{
		if (!TryGetAccount(accountId, out AccountInstance? account))
		{
			throw new KeyNotFoundException($"Could not find account {accountId}");
		}

		return account.Serialize();
	}

	public void ImportAccountData(string accountData)
	{
		SerializedAccountData? data = JsonConvert.DeserializeObject<SerializedAccountData>(accountData);
		if (data == null || data.Info == null)
		{
			throw new InvalidOperationException("Invalid account data");
		}
		if (!Storage.Write(new AccountDataContext(data.Info.Id), "info.json", data.Info))
		{
			throw new InvalidOperationException("Failed to write account info");
		}
		AccountInstance account = LoadAccount(data.Info.Id);
		account.Deserialize(data);
	}
}
