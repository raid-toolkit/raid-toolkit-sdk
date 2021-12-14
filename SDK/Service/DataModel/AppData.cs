using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Raid.Service
{
    public class AppData
    {
        private readonly string m_storagePath;
        private readonly string m_accountsPath;
        private readonly string m_staticDataPath;
        private readonly string m_settingsFilePath;
        private Dictionary<string, UserAccount> m_userAccounts;
        private readonly IServiceProvider ServiceProvider;
        public IEnumerable<UserAccount> UserAccounts => m_userAccounts.Values;

        public AppData(IOptions<AppSettings> settings, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            m_storagePath = settings.Value.StorageLocation ?? "data";
            if (!Path.IsPathFullyQualified(m_storagePath))
            {
                m_storagePath = Path.Join(AppConfiguration.InstallationPath, m_storagePath);
            }

            // create basic directories
            m_accountsPath = Path.Join(m_storagePath, "accounts");
            _ = Directory.CreateDirectory(m_accountsPath);
            m_staticDataPath = Path.Join(m_storagePath, "staticData");
            _ = Directory.CreateDirectory(m_staticDataPath);
            m_settingsFilePath = Path.Join(m_storagePath, ".settings");
        }

        public void Load()
        {
            // enumerate accounts
            m_userAccounts = Directory.GetDirectories(m_accountsPath)
                .ToDictionary(
                    id => Path.GetFileName(id),
                    id => new UserAccount(Path.GetFileName(id), ServiceProvider.CreateScope())
                );
            foreach (UserAccount account in m_userAccounts.Values)
            {
                account.Load();
            }
        }

        public UserAccount GetAccount(string id)
        {
            if (!m_userAccounts.TryGetValue(id, out UserAccount account))
            {
                account = new UserAccount(id, ServiceProvider.CreateScope());
                account.Load();
                m_userAccounts.Add(id, account);
            }
            return account;
        }

        public UserSettings ReadUserSettings()
        {
            if (!File.Exists(m_settingsFilePath))
            {
                return new UserSettings();
            }
            try
            {
                return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(m_settingsFilePath));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteUserSettings(UserSettings settings)
        {
            _ = Directory.CreateDirectory(Path.GetDirectoryName(m_settingsFilePath));
            File.WriteAllText(m_settingsFilePath, JsonConvert.SerializeObject(settings));
        }

        public T ReadStaticData<T>(string key) where T : class
        {
            string filePath = Path.Join(m_staticDataPath, key);
            if (!File.Exists(filePath))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteStaticData<T>(string key, T value) where T : class
        {
            string filePath = Path.Join(m_staticDataPath, key);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
        }

        public T ReadAccountData<T>(string userId, string key) where T : class
        {
            string filePath = Path.Join(m_accountsPath, userId, key);
            if (!File.Exists(filePath))
            {
                return null;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteAccountData<T>(string userId, string key, T value) where T : class
        {
            string filePath = Path.Join(m_accountsPath, userId, key);
            _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
        }
    }
}
