using System;
using System.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.DataServices
{
    public class CachedDataStorage : IDataStorage
    {
        private static readonly object EmptyObject = new();
        private static readonly ConcurrentDictionary<string, object> Cache = new();

        private readonly IDataStorage UnderlyingStorage;

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public IEnumerable<string> Keys
        {
            get
            {
                // TOTAL ABSOLUTE HACKERY
                return Cache.Keys.ToArray()
                    .Select(key => key.Split(';'))
                    .Where(parts => parts.Length == 3)
                    .Select(parts => parts[1])
                    .Distinct();
            }
        }

        public CachedDataStorage()
        { }

        protected CachedDataStorage(IDataStorage underlyingStorage)
        {
            UnderlyingStorage = underlyingStorage;
        }

        public bool TryRead<T>(IDataContext context, string key, out T value) where T : class
        {
            string cacheKey = string.Join(";", context.Parts.Concat(new[] { key }).ToArray());
            object cacheEntry = Cache.GetOrAdd(cacheKey, cacheKey => ReadFromUnderlyingStorage<T>(context, key));
            if (cacheEntry == EmptyObject)
            {
                value = default;
                return false;
            }
            value = (T)cacheEntry;
            return true;
        }

        public bool Write<T>(IDataContext context, string key, T value) where T : class
        {
            string cacheKey = string.Join(";", context.Parts.Concat(new[] { key }).ToArray());
            T updatedValue = Cache.AddOrUpdate(cacheKey, _ => value, (_1, oldValue) => UpdateAndWriteIfChanged(oldValue, value)) as T;
            // only write if the new value was added
            if (updatedValue == value)
            {
                _ = (UnderlyingStorage?.Write(context, key, value));
                Updated?.Invoke(this, new DataStorageUpdatedEventArgs(key, value));
                return true;
            }
            return false;
        }

        private object ReadFromUnderlyingStorage<T>(IDataContext context, string key) where T : class
        {
            return UnderlyingStorage != null && UnderlyingStorage.TryRead(context, key, out T value) ? value : EmptyObject;
        }

        private static T UpdateAndWriteIfChanged<T>(T oldValue, T newValue)
        {
            // TODO: remove this extra serialization, and persist it directly as a string if possible
            return JsonConvert.SerializeObject(oldValue) == JsonConvert.SerializeObject(newValue) ? oldValue : newValue;
        }
    }

    public class CachedDataStorage<T> : CachedDataStorage where T : class, IDataStorage
    {
        public CachedDataStorage(T provider)
        : base(provider)
        { }
    }
}
