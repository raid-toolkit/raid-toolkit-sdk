using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Raid.Service.DataModel
{
    public class UserData
    {
        #region Static init
        private static readonly UserData s_instance;
        private static readonly StorageSettings s_settings;
        public static UserData Instance => s_instance;
        static UserData()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            s_settings = config.GetSection("storage").Get<StorageSettings>();
            s_instance = new UserData();
        }
        #endregion

        private readonly string m_storagePath;
        private readonly string m_accountsPath;
        private readonly string m_staticDataPath;
        private readonly Dictionary<string, UserAccount> m_userAccounts;

        public IEnumerable<UserAccount> UserAccounts => m_userAccounts.Values;

        private UserData()
        {
            m_storagePath = s_settings.StorageLocation ?? "data";
            if (!Path.IsPathFullyQualified(m_storagePath))
            {
                m_storagePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, m_storagePath);
            }

            // create basic directories
            m_accountsPath = Path.Join(m_storagePath, "accounts");
            Directory.CreateDirectory(m_accountsPath);
            m_staticDataPath = Path.Join(m_storagePath, "staticData");
            Directory.CreateDirectory(m_staticDataPath);

            // enumerate accounts
            m_userAccounts = Directory.GetDirectories(m_accountsPath).ToDictionary(id => Path.GetFileName(id), id => new UserAccount(Path.GetFileName(id)));
        }

        public UserAccount GetAccount(string id)
        {
            if (!m_userAccounts.TryGetValue(id, out UserAccount account))
            {
                account = new UserAccount(id);
                m_userAccounts.Add(id, account);
            }
            return account;
        }

        public T ReadStaticData<T>(string key) where T : class
        {
            string filePath = Path.Join(m_staticDataPath, key);
            if (!File.Exists(filePath))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public void WriteStaticData<T>(string key, T value) where T : class
        {
            string filePath = Path.Join(m_staticDataPath, key);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
        }

        public T ReadAccountData<T>(string userId, string key) where T : class
        {
            string filePath = Path.Join(m_accountsPath, userId, key);
            if (!File.Exists(filePath))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public void WriteAccountData<T>(string userId, string key, T value) where T : class
        {
            string filePath = Path.Join(m_accountsPath, userId, key);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
        }
    }
}