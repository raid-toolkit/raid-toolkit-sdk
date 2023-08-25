using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Implementations;

namespace Raid.Toolkit.Extensibility.Host
{
    public class AccountInstance : IAccount
    {
        private readonly object _syncRoot = new();
        private Dictionary<IAccountExtensionFactory, ExtensionOwnedValue<IAccountExtension>> Extensions = new();
        private IGameInstance? GameInstance;
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

        private ExtensionOwnedValue<IAccountExtension>[] GetExtensionsSnapshot()
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
            if (!Storage.TryRead(Context, "info.json", out AccountBase accountInfo))
            {
                throw new KeyNotFoundException("Account info not found");
            }
            _AccountInfo = accountInfo;
        }

        public string Serialize()
        {
            SerializedAccountData data = new(AccountInfo);

            ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
            foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension.Value is IAccountExportable exportable)
                {
                    IAccountReaderWriter readerWriter = data.CreateReaderWriter(new ExtensionDataContext(Context, extension.Manifest.Id));
                    exportable.Export(readerWriter);
                }
            }
            return JsonConvert.SerializeObject(data);
        }

        public void Deserialize(SerializedAccountData data)
        {
            ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
            foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension.Value is IAccountExportable exportable)
                {
                    IAccountReaderWriter readerWriter = data.CreateReaderWriter(new ExtensionDataContext(Context, extension.Manifest.Id));
                    exportable.Import(readerWriter);
                }
            }
        }

        public bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class
        {
            ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
            foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension.Value is IAccountPublicApi<T> publicApi)
                {
                    api = publicApi.GetApi();
                    return true;
                }
            }
            api = null;
            return false;
        }

        public async Task Tick()
        {
            ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
            foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension.Value is IAccountExtensionService service)
                    await service.OnTick();
            }
        }

        public void OnConnected(IGameInstance gameInstance)
        {
            lock (_syncRoot)
            {
                GameInstance = gameInstance;
                ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
                foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
                {
                    lock (_syncRoot)
                        if (!Extensions.ContainsValue(extension))
                            continue; // extension was since removed

                    extension.Value.OnConnected(gameInstance.Runtime);
                }
            }
        }

        public void OnDisconnected()
        {
            lock (_syncRoot)
            {
                GameInstance = null;
                ExtensionOwnedValue<IAccountExtension>[] extensions = GetExtensionsSnapshot();
                foreach (ExtensionOwnedValue<IAccountExtension> extension in extensions)
                {
                    lock (_syncRoot)
                        if (!Extensions.ContainsValue(extension))
                            continue; // extension was since removed

                    extension.Value.OnDisconnected();
                }
            }
        }

        public void AddExtension(ExtensionManifest manifest, IAccountExtensionFactory factory)
        {
            IAccountExtension extension = factory.Create(this);
            lock (_syncRoot)
            {
                Extensions.Add(factory, new(manifest, extension));
                if (GameInstance != null)
                    extension.OnConnected(GameInstance.Runtime);
            }
        }

        public void RemoveExtension(IAccountExtensionFactory factory)
        {
            bool removed = false;
            ExtensionOwnedValue<IAccountExtension>? extension;

            lock (_syncRoot)
            {
                removed = Extensions.Remove(factory, out extension);
            }

            if (removed && extension != null)
            {
                if (IsOnline)
                    extension.Value.OnDisconnected();

                if (extension.Value is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}
