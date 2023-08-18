using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host
{
    public class AccountInstance : IAccount
    {
        private readonly object _syncRoot = new();
        private Dictionary<IAccountExtensionFactory, IAccountExtension> Extensions = new();
        private IGameInstance? GameInstance;
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly ILogger<AccountInstance> Logger;
        private readonly AccountDataContext Context;
        private readonly AccountBase AccountInfo;

        public string Id { get; }
        public string AccountName => AccountInfo.Name;
        public string AvatarUrl => AccountInfo.AvatarUrl;
        public bool IsOnline => GameInstance != null;
        public Il2CsRuntimeContext? Runtime => GameInstance?.Runtime;

        private IAccountExtension[] GetExtensionsSnapshot()
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

            if (!Storage.TryRead(Context, "info.json", out AccountInfo))
            {
                throw new KeyNotFoundException("Account info not found");
            }
        }

        public bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class
        {
            IAccountExtension[] extensions = GetExtensionsSnapshot();
            foreach (IAccountExtension extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension is IAccountPublicApi<T> publicApi)
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
            IAccountExtension[] extensions = GetExtensionsSnapshot();
            foreach (IAccountExtension extension in extensions)
            {
                lock (_syncRoot)
                    if (!Extensions.ContainsValue(extension))
                        continue; // extension was since removed

                if (extension is IAccountExtensionService service)
                    await service.OnTick();
            }
        }

        public void OnConnected(IGameInstance gameInstance)
        {
            lock (_syncRoot)
            {
                GameInstance = gameInstance;
                IAccountExtension[] extensions = GetExtensionsSnapshot();
                foreach (IAccountExtension extension in extensions)
                {
                    lock (_syncRoot)
                        if (!Extensions.ContainsValue(extension))
                            continue; // extension was since removed

                    extension.OnConnected(gameInstance.Runtime);
                }
            }
        }

        public void OnDisconnected()
        {
            lock (_syncRoot)
            {
                GameInstance = null;
                IAccountExtension[] extensions = GetExtensionsSnapshot();
                foreach (IAccountExtension extension in extensions)
                {
                    lock (_syncRoot)
                        if (!Extensions.ContainsValue(extension))
                            continue; // extension was since removed

                    extension.OnDisconnected();
                }
            }
        }

        public void AddExtension(IAccountExtensionFactory factory)
        {
            IAccountExtension extension = factory.Create(this);
            lock (_syncRoot)
            {
                Extensions.Add(factory, extension);
                if (GameInstance != null)
                    extension.OnConnected(GameInstance.Runtime);
            }
        }

        public void RemoveExtension(IAccountExtensionFactory factory)
        {
            bool removed = false;
            IAccountExtension? extension;

            lock (_syncRoot)
            {
                removed = Extensions.Remove(factory, out extension);
            }

            if (removed && extension != null)
            {
                if (IsOnline)
                    extension.OnDisconnected();

                if (extension is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}
