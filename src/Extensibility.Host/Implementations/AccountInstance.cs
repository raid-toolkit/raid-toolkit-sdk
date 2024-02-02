using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host;

internal class AccountExtensionState
{
	private readonly object _syncRoot = new();
	private readonly ExtensionOwnedValue<IAccountExtension> ExtensionValue;
	private readonly IAccount Account;
	private readonly ILogger Logger;
	public IAccountExtension Extension => ExtensionValue.Value;
	public PackageManifest Owner => ExtensionValue.Manifest;
	private readonly string ExtensionTypeName;

	private Task? CurrentTick;

	public AccountExtensionState(ExtensionOwnedValue<IAccountExtension> extension, IAccount account, ILogger logger)
	{
		ExtensionValue = extension;
		Account = account;
		Logger = logger;
		ExtensionTypeName = ExtensionValue.Value.GetType().FullName ?? "unknown";
	}

	public void Tick()
	{
		if (Extension is not IAccountExtensionService service)
			return;

		lock (_syncRoot)
		{
			if ((CurrentTick == null || CurrentTick.IsCompleted) && service.HasWork)
			{
				CurrentTick = Task.Run(async () =>
				{
					Stopwatch swScoped = Stopwatch.StartNew();
					try
					{
						//Logger.LogInformation("Executing background work Account:{account} Extension:{extension}", Account.Id, ExtensionTypeName);
						await service.OnTick();
						//Logger.LogInformation("End background processing for Account:{account} Extension:{extension} Elapsed:{ms}ms", Account.Id, ExtensionTypeName, swScoped.ElapsedMilliseconds);
					}
					catch (Exception ex)
					{
						Logger.LogError(ex, "Failed background processing for Account:{account} Extension:{extension} Elapsed:{ms}ms", Account.Id, ExtensionTypeName, swScoped.ElapsedMilliseconds);
					}
					finally
					{
						CurrentTick = null;
					}
				});
			}
		}

	}
}
public class AccountInstance : IAccount
{
	private readonly object _syncRoot = new();
	private readonly Dictionary<IAccountExtensionFactory, AccountExtensionState> Extensions = new();
	private ILoadedGameInstance? GameInstance;
	private readonly CachedDataStorage<PersistedDataStorage> Storage;
	private readonly ILogger<AccountInstance> Logger;
	private readonly AccountDataContext Context;
	private AccountBase? _AccountInfo;

	public string Id { get; }
	public AccountBase AccountInfo => _AccountInfo ?? throw new InvalidOperationException($"Account is not yet loaded.");
	public string AccountName => AccountInfo.Name;
	public string AvatarUrl => AccountInfo.AvatarUrl;
	public bool IsOnline => GameInstance != null;
	public Il2CsRuntimeContext? Runtime => GameInstance?.Runtime;

	public event EventHandler<AccountEventArgs>? OnConnected;
	public event EventHandler<AccountEventArgs>? OnDisconnected;

	private AccountExtensionState[] GetExtensionsSnapshot()
	{
		lock (_syncRoot)
			return Extensions.Values.ToArray();
	}

	public AccountInstance(string id,
		ILogger<AccountInstance> logger,
		CachedDataStorage<PersistedDataStorage> storage)
	{
		Id = id;
		Logger = logger;
		Storage = storage;
		Context = id;
		LoadFromStorage();
	}

	public void LoadFromStorage()
	{
		if (!Storage.TryRead(Context, "info.json", out AccountBase? accountInfo))
		{
			throw new KeyNotFoundException("Account info not found");
		}
		_AccountInfo = accountInfo;
	}

	public string Serialize()
	{
		SerializedAccountData data = new(AccountInfo);

		AccountExtensionState[] extensions = GetExtensionsSnapshot();
		foreach (AccountExtensionState extension in extensions)
		{
			lock (_syncRoot)
				if (!Extensions.ContainsValue(extension))
					continue; // extension was since removed

			if (extension.Extension is IAccountExportable exportable)
			{
				IAccountReaderWriter readerWriter = data.CreateReaderWriter(new ExtensionDataContext(Context, extension.Owner.Id));
				exportable.Export(readerWriter);
			}
		}
		return JsonConvert.SerializeObject(data);
	}

	public void Deserialize(SerializedAccountData data)
	{
		AccountExtensionState[] extensions = GetExtensionsSnapshot();
		foreach (AccountExtensionState extension in extensions)
		{
			lock (_syncRoot)
				if (!Extensions.ContainsValue(extension))
					continue; // extension was since removed

			if (extension.Extension is IAccountExportable exportable)
			{
				IAccountReaderWriter readerWriter = data.CreateReaderWriter(new ExtensionDataContext(Context, extension.Owner.Id));
				exportable.Import(readerWriter);
			}
		}
	}

	public bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class
	{
		AccountExtensionState[] extensions = GetExtensionsSnapshot();
		foreach (AccountExtensionState extension in extensions)
		{
			lock (_syncRoot)
				if (!Extensions.ContainsValue(extension))
					continue; // extension was since removed

			if (extension.Extension is IAccountPublicApi<T> publicApi)
			{
				api = publicApi.GetApi();
				return true;
			}
		}
		api = null;
		return false;
	}

	public void Tick()
	{
		AccountExtensionState[] extensions = GetExtensionsSnapshot();
		foreach (AccountExtensionState extension in extensions)
		{
			lock (_syncRoot)
				if (!Extensions.ContainsValue(extension))
					return; // extension was since removed

			extension.Tick();
		}
	}

	public void Connect(ILoadedGameInstance gameInstance)
	{
		lock (_syncRoot)
		{
			if (GameInstance == gameInstance)
				return;

			GameInstance = gameInstance;
			AccountExtensionState[] extensions = GetExtensionsSnapshot();
			foreach (AccountExtensionState extension in extensions)
			{
				lock (_syncRoot)
					if (!Extensions.ContainsValue(extension))
						continue; // extension was since removed

				if (gameInstance.Runtime == null)
					continue;
				extension.Extension.OnConnected(gameInstance.Runtime);
			}
			OnConnected?.Invoke(this, new AccountEventArgs());
		}
	}

	public void Disconnect()
	{
		lock (_syncRoot)
		{
			OnDisconnected?.Invoke(this, new AccountEventArgs());
			GameInstance = null;
			AccountExtensionState[] extensions = GetExtensionsSnapshot();
			foreach (AccountExtensionState extension in extensions)
			{
				lock (_syncRoot)
					if (!Extensions.ContainsValue(extension))
						continue; // extension was since removed

				extension.Extension.OnDisconnected();
			}
		}
	}

	public void AddExtension(PackageManifest manifest, IAccountExtensionFactory factory)
	{
		IAccountExtension extension = factory.Create(this);
		lock (_syncRoot)
		{
			Extensions.Add(factory, new(new(manifest, extension), this, Logger));
			if (GameInstance?.Runtime != null)
				extension.OnConnected(GameInstance.Runtime);
		}
	}

	public void RemoveExtension(IAccountExtensionFactory factory)
	{
		bool removed = false;
		AccountExtensionState? extension;

		lock (_syncRoot)
		{
			removed = Extensions.Remove(factory, out extension);
		}

		if (removed && extension != null)
		{
			if (IsOnline)
				extension.Extension.OnDisconnected();

			if (extension.Extension is IDisposable disposable)
				disposable.Dispose();
		}
	}
}

