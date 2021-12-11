using System;
using System.IO;
using Newtonsoft.Json;
using Raid.Common;

namespace Raid.DataServices
{

    public class PersistedDataStorage : IDataStorage
    {
        private readonly string StoragePath;
        private IDataContext DataContext;

        public PersistedDataStorage()
        {
            StoragePath = Path.Join(RegistrySettings.InstallationPath, "data");
        }

        public void SetContext(IDataContext context)
        {
            if (DataContext != null)
                throw new InvalidOperationException("Already set context");

            DataContext = context;
            _ = Directory.CreateDirectory(Path.Join(StoragePath, Path.Join(DataContext.Parts)));
        }

        public event EventHandler<DataStorageUpdatedEventArgs> Updated;

        public bool TryRead<T>(string key, out T value) where T : class
        {
            string filePath = Path.Join(StoragePath, Path.Join(DataContext.Parts), key);
            if (!File.Exists(filePath))
            {
                value = default;
                return false;
            }
            try
            {
                value = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        public bool Write<T>(string key, T value) where T : class
        {
            string filePath = Path.Join(StoragePath, Path.Join(DataContext.Parts), key);
            string data = JsonConvert.SerializeObject(value);
            File.WriteAllText(filePath, data);
            return true;
        }
    }
}
