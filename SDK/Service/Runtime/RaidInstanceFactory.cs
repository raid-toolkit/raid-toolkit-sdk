using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.Service
{
    public class RaidInstanceFactory
    {
        public readonly ConcurrentDictionary<int, RaidInstance> Instances = new();
        private readonly IReadOnlyList<IDependencyTypeFactory<IAccountFacet>> AccountFacets;
        private readonly UserData UserData;
        private readonly StaticDataCache StaticDataCache;

        public RaidInstanceFactory(
            IEnumerable<IDependencyTypeFactory<IAccountFacet>> accountFacets,
            UserData userData,
            StaticDataCache staticDataCache) =>
            (AccountFacets, UserData, StaticDataCache) = (accountFacets.ToList(), userData, staticDataCache);

        public RaidInstance GetById(string id)
        {
            return Instances.Single(instance => instance.Value.Id == id).Value;
        }

        public RaidInstance Create(Process process, IServiceScope scope)
        {
            process.MainModule.FileName.ToString();
            RaidInstance instance = scope.ServiceProvider.GetService<RaidInstance>().Attach(process);
            Instances.TryAdd(process.Id, instance);
            return instance;
        }

        public void Destroy(int processId)
        {
            if (Instances.TryGetValue(processId, out RaidInstance instance))
            {
                instance.Dispose();
                Instances.TryRemove(new(processId, instance));
            }
        }
    }
}