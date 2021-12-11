using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Raid.DataServices
{
    public class CachedDataStorage : IDataStorage
    {
        private static readonly object EmptyObject = new();
        private readonly IDataStorage UnderlyingStorage;
        private readonly ConcurrentDictionary<string, object> Cache = new();

        public CachedDataStorage()
        { }

        protected CachedDataStorage(IDataStorage underlyingStorage)
        {
            UnderlyingStorage = underlyingStorage;
        }

        public bool TryRead<T>(string key, out T value)
        {
            object cacheEntry = Cache.GetOrAdd(key, ReadFromUnderlyingStorage<T>);
            if (cacheEntry == EmptyObject)
            {
                value = default;
                return false;
            }
            value = (T)cacheEntry;
            return true;
        }

        private object ReadFromUnderlyingStorage<T>(string key)
        {
            return UnderlyingStorage != null && UnderlyingStorage.TryRead<T>(key, out T value) ? value : EmptyObject;
        }

        public void Write<T>(string key, T value)
        {
            _ = Cache.AddOrUpdate(key, _ => value, (_1, _2) => value);
            UnderlyingStorage?.Write<T>(key, value);
        }
    }

    public class CachedDataStorage<T> : CachedDataStorage where T : IDataStorage
    {
        public CachedDataStorage(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<T>())
        { }
    }
}
