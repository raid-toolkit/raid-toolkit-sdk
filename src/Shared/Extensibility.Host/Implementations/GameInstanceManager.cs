using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Raid.Toolkit.Extensibility.Host
{
    public class GameInstanceManager : IGameInstanceManager
    {
        private readonly ConcurrentDictionary<int, IGameInstance> _Instances = new();
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
            PersistedDataManager<AccountDataContext> accountDataManager)
        {
            ServiceProvider = serviceProvider;
            ContextDataManager = contextDataManager;
            AccountDataManager = accountDataManager;
            StaticDataManager = staticDataManager;
        }

        public void Update()
        {
            foreach (var instance in _Instances.Values)
            {
                if (!HasCheckedStaticData)
                {
                    var result = StaticDataManager.Update(instance.Runtime, StaticDataContext.Default);
                    if (result == UpdateResult.Failed)
                        continue;

                    HasCheckedStaticData = true;
                }

                AccountDataManager.Update(instance.Runtime, instance.Id);
            }
        }

        public IGameInstance GetById(string id)
        {
            return Instances.FirstOrDefault(instance => instance.Id == id);
        }

        public void AddInstance(Process process)
        {
            IGameInstance instance = ActivatorUtilities.CreateInstance<GameInstance>(ServiceProvider, process);
            _Instances.TryAdd(instance.Token, instance);
            OnAdded?.Invoke(this, new(instance));
        }

        public void RemoveInstance(int token)
        {
            if (_Instances.TryRemove(token, out IGameInstance instance))
            {
                OnRemoved?.Invoke(this, new(instance));
                instance.Dispose();
            }
        }
    }
}
