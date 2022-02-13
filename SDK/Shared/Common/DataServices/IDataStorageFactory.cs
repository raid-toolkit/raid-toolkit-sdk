using System;
using System.Collections.Concurrent;

namespace Raid.DataServices
{
    public interface IDataStorageFactory<out TFactory> where TFactory : IDataStorage
    {
        IDataStorage GetStorage(IDataContext context);
    }

    public class DataStorageFactoryManager<TFactory> : IDataStorageFactory<TFactory> where TFactory : class, IDataStorage, new()
    {
        private readonly ConcurrentDictionary<string, IDataStorage> StorageMap = new();
        private readonly IServiceProvider ServiceProvider;
        public DataStorageFactoryManager(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IDataStorage GetStorage(IDataContext context)
        {
            return StorageMap.GetOrAdd(string.Join("|", context.Parts), (key) =>
            {
                var factory = new TFactory();
                factory.SetContext(context, ServiceProvider);
                return factory;
            });
        }
    }
}