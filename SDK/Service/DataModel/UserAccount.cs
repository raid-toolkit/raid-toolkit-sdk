using System.Collections.Generic;

namespace Raid.Service.DataModel
{
    public class UserAccount : IModelDataSource
    {
        private Dictionary<string, object> m_data = new();
        private string m_userId;
        public UserAccount(string userId)
        {
            m_userId = userId;
        }

        public T Get<T>(string key) where T : class
        {
            if (!m_data.TryGetValue(key, out object value))
            {
                value = UserData.Instance.ReadAccountData<T>(m_userId, key);
                m_data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            m_data[key] = value;
            UserData.Instance.WriteAccountData<T>(m_userId, key, value);
        }
    }
}
