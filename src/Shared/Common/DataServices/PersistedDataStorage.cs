using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Raid.Toolkit.Common;

namespace Raid.DataServices
{

    public class PersistedDataStorage : IDataStorage
    {
        private readonly string StoragePath;
        private IDataContext_deprecated DataContext;
        private IDataStorageReaderWriter Storage;
        public IEnumerable<string> Keys => throw new NotSupportedException();

        public PersistedDataStorage()
        {
            StoragePath = Path.Combine(RegistrySettings.InstallationPath, "data");
        }

        public void SetContext(IDataContext_deprecated context, IServiceProvider serviceProvider)
        {
            if (DataContext != null)
                throw new InvalidOperationException("Already set context");

            DataContext = context;
            _ = Directory.CreateDirectory(Path.Combine(StoragePath, Path.Combine(DataContext.Parts)));

            Storage = serviceProvider.GetRequiredService<IDataStorageReaderWriter>();
        }

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public bool TryRead<T>(string key, out T value) where T : class
        {
            string filePath = Path.Combine(StoragePath, Path.Combine(DataContext.Parts), key);
            return Storage.TryRead(filePath, out value);
        }

        public bool Write<T>(string key, T value) where T : class
        {
            string filePath = Path.Combine(StoragePath, Path.Combine(DataContext.Parts), key);
            Updated?.Invoke(this, new DataStorageUpdatedEventArgs(key, value));
            return Storage.Write(filePath, value);
        }
    }
}
