using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extensibility.Host
{
    public class GameInstanceManager : IGameInstanceManager
    {
        private readonly ConcurrentDictionary<int, IGameInstance> _Instances = new();
        private readonly ConcurrentDictionary<int, IGameInstance> _RawInstances = new();
        private readonly IServiceProvider ServiceProvider;
        private readonly IContextDataManager ContextDataManager;
        private readonly PersistedDataManager<StaticDataContext> StaticDataManager;
        private readonly PersistedDataManager<AccountDataContext> AccountDataManager;
        private bool HasCheckedStaticData;

        public IReadOnlyList<IGameInstance> Instances => _Instances.Values.ToList();
        public event EventHandler<IGameInstanceManager.GameInstancesUpdatedEventArgs> OnAdded;
        public event EventHandler<IGameInstanceManager.GameInstancesUpdatedEventArgs> OnRemoved;

        public GameInstanceManager(
            IServiceProvider serviceProvider,
            IContextDataManager contextDataManager,
            PersistedDataManager<StaticDataContext> staticDataManager,
            PersistedDataManager<AccountDataContext> accountDataManager,
            IHostApplicationLifetime lifetime)
        {
            ServiceProvider = serviceProvider;
            ContextDataManager = contextDataManager;
            AccountDataManager = accountDataManager;
            StaticDataManager = staticDataManager;
            _ = lifetime.ApplicationStopped.Register(() =>
            {
                int[] instanceKeys = _RawInstances.Keys.ToArray();
                foreach (int key in instanceKeys)
                {
                    _ = _RawInstances.Remove(key, out IGameInstance instance);
                    _ = _Instances.Remove(key, out _);
                    instance.Dispose();
                }
            });
        }

        public IGameInstance GetById(string id)
        {
            return Instances.FirstOrDefault(instance => instance.Id == id);
        }

        public void AddInstance(Process process)
        {
            IGameInstance instance = _RawInstances.GetOrAdd(process.Id, (token) => ActivatorUtilities.CreateInstance<GameInstance>(ServiceProvider, process));
            instance.InitializeOrThrow(process);

            _ = _Instances.TryAdd(instance.Token, instance);
            OnAdded?.Raise(this, new(instance));
        }

        public void RemoveInstance(int token)
        {
            if (_RawInstances.TryRemove(token, out IGameInstance instance))
            {
                _ = _Instances.TryRemove(token, out _);
                OnRemoved?.Raise(this, new(instance));
                instance.Dispose();
            }
        }
    }
}
