using Client.Model;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Extensibility.Host;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Raid.Toolkit.Extensibility.Shared
{
    public class GameInstanceManager : IGameInstanceManager
    {
        private readonly ConcurrentDictionary<int, IGameInstance> _Instances = new();
        private readonly IServiceProvider ServiceProvider;

        public IReadOnlyList<IGameInstance> Instances => _Instances.Values.ToList();
        public GameInstanceManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void AddInstance(Process process)
        {
            IGameInstance instance = ActivatorUtilities.CreateInstance<GameInstance>(ServiceProvider, process);
            _Instances.TryAdd(instance.Token, instance);
        }

        public void RemoveInstance(int token)
        {
            if (_Instances.TryRemove(token, out IGameInstance instance))
            {
                instance.Dispose();
            }
        }
    }
}
