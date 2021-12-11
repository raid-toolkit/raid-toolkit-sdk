using System;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Raid.Common;

namespace Raid.DataServices
{
    public class PersistedDataStorage : IDataStorage
    {
        private readonly string StoragePath;

        public PersistedDataStorage(IOptions<DataServicesSettings> settings)
        {
            StoragePath = settings.Value.StorageLocation ?? "data";
            if (!Path.IsPathFullyQualified(StoragePath))
            {
                StoragePath = Path.Join(RegistrySettings.InstallationPath, StoragePath);
            }
            _ = Directory.CreateDirectory(StoragePath);
        }

        public bool TryRead<T>(string key, out T value)
        {
            string filePath = Path.Join(StoragePath, key);
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

        public void Write<T>(string key, T value)
        {
            string filePath = Path.Join(StoragePath, key);
            string data = JsonConvert.SerializeObject(value);
            File.WriteAllText(filePath, data);
        }
    }
}
