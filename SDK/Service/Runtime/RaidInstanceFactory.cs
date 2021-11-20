using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.Service
{
    public class RaidInstanceFactory
    {
        public readonly ConcurrentDictionary<int, RaidInstance> Instances = new();

        public RaidInstance GetById(string id)
        {
            return Instances.Single(instance => instance.Value.Id == id).Value;
        }

        public RaidInstance Create(Process process, IServiceScope scope)
        {
            _ = process.MainModule.FileName.ToString();
            RaidInstance instance = scope.ServiceProvider.GetService<RaidInstance>().Attach(process);
            _ = Instances.TryAdd(process.Id, instance);
            return instance;
        }

        public void Destroy(int processId)
        {
            if (Instances.TryGetValue(processId, out RaidInstance instance))
            {
                instance.Dispose();
                _ = Instances.TryRemove(new(processId, instance));
            }
        }
    }
}
