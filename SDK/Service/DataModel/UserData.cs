using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Raid.Service
{
    public class UserData
    {
        private readonly string m_storagePath;
        private readonly string m_accountsPath;
        private readonly string m_staticDataPath;
        private readonly Dictionary<string, UserAccount> m_userAccounts;

        public IEnumerable<UserAccount> UserAccounts => m_userAccounts.Values;

        public UserData(IOptions<AppSettings> settings)
        {
            m_storagePath = settings.Value.StorageLocation ?? "data";
            if (!Path.IsPathFullyQualified(m_storagePath))
            {
                m_storagePath = Path.Join(AppConfiguration.ExecutableDirectory, m_storagePath);
            }

            // create basic directories
            m_accountsPath = Path.Join(m_storagePath, "accounts");
            Directory.CreateDirectory(m_accountsPath);
            m_staticDataPath = Path.Join(m_storagePath, "staticData");
            Directory.CreateDirectory(m_staticDataPath);

            // enumerate accounts
            m_userAccounts = Directory.GetDirectories(m_accountsPath).ToDictionary(id => Path.GetFileName(id), id => new UserAccount(Path.GetFileName(id), this));
        }

        public UserAccount GetAccount(string id)
        {
            if (!m_userAccounts.TryGetValue(id, out UserAccount account))
            {
                account = new UserAccount(id, this);
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
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
        }
    }
}