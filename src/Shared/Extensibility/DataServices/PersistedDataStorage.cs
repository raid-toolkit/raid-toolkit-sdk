using System;
using System.Collections.Generic;
using System.IO;

namespace Raid.Toolkit.Extensibility.DataServices
{
    public class PersistedDataStorage : IDataStorage
    {
        private readonly string StoragePath;
        private IDataStorageReaderWriter Storage;

        public PersistedDataStorage(IDataStorageReaderWriter storage, IDataServiceSettings settings)
        {
            Storage = storage;
            StoragePath = settings.StoragePath;
        }

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public IEnumerable<string> Keys => throw new NotSupportedException();

        public bool TryRead<T>(IDataContext context, string key, out T value) where T : class
        {
            string filePath = Path.Combine(StoragePath, Path.Combine(context.Parts), key);
            return Storage.TryRead(filePath, out value);
        }

        public bool Write<T>(IDataContext context, string key, T value) where T : class
        {
            string filePath = Path.Combine(StoragePath, Path.Combine(context.Parts), key);
            Updated?.Invoke(this, new DataStorageUpdatedEventArgs(context, key, value));
            return Storage.Write(filePath, value);
        }
    }
}
