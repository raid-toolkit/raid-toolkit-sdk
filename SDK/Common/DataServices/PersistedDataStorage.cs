using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Raid.Common;

namespace Raid.DataServices
{

    public class PersistedDataStorage : IDataStorage
    {
        private readonly string StoragePath;
        private IDataContext DataContext;
        private IDataStorageReaderWriter Storage;

        public PersistedDataStorage()
        {
            StoragePath = Path.Join(RegistrySettings.InstallationPath, "data");
        }

        public void SetContext(IDataContext context, IServiceProvider serviceProvider)
        {
            if (DataContext != null)
                throw new InvalidOperationException("Already set context");

            DataContext = context;
            _ = Directory.CreateDirectory(Path.Join(StoragePath, Path.Join(DataContext.Parts)));

            Storage = serviceProvider.GetRequiredService<IDataStorageReaderWriter>();
        }

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public bool TryRead<T>(string key, out T value) where T : class
        {
            string filePath = Path.Join(StoragePath, Path.Join(DataContext.Parts), key);
            return Storage.TryRead(filePath, out value);
        }

        public bool Write<T>(string key, T value) where T : class
        {
            string filePath = Path.Join(StoragePath, Path.Join(DataContext.Parts), key);
            Updated?.Invoke(this, new DataStorageUpdatedEventArgs(key, value));
            return Storage.Write(filePath, value);
        }
    }
}
