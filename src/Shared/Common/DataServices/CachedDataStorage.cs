using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Raid.DataServices
{
    public class CachedDataStorage : IDataStorage
    {
        private static readonly object EmptyObject = new();
        private readonly IDataStorage UnderlyingStorage;
        private readonly ConcurrentDictionary<string, object> Cache = new();

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public CachedDataStorage()
        { }

        protected CachedDataStorage(IDataStorage underlyingStorage)
        {
            UnderlyingStorage = underlyingStorage;
        }

        public void SetContext(IDataContext context, IServiceProvider serviceProvider)
        {
            UnderlyingStorage?.SetContext(context, serviceProvider);
        }

        public bool TryRead<T>(string key, out T value) where T : class
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

        private object ReadFromUnderlyingStorage<T>(string key) where T : class
        {
            return UnderlyingStorage != null && UnderlyingStorage.TryRead(key, out T value) ? value : EmptyObject;
        }

        public bool Write<T>(string key, T value) where T : class
        {
            T updatedValue = Cache.AddOrUpdate(key, _ => value, (_1, oldValue) => UpdateAndWriteIfChanged(oldValue, value)) as T;
            // only write if the new value was added
            if (updatedValue == value)
            {
                _ = (UnderlyingStorage?.Write(key, value));
                Updated?.Invoke(this, new DataStorageUpdatedEventArgs(key, value));
                return true;
            }
            return false;
        }

        private static T UpdateAndWriteIfChanged<T>(T oldValue, T newValue)
        {
            // TODO: remove this extra serialization, and persist it directly as a string if possible
            return JsonConvert.SerializeObject(oldValue) == JsonConvert.SerializeObject(newValue) ? oldValue : newValue;
        }

        public void SetContext(IDataContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class CachedDataStorage<T> : CachedDataStorage where T : class, IDataStorage, new()
    {
        public CachedDataStorage()
        : base(new T())
        { }
    }
}
