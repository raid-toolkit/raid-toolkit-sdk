using System.Collections.Generic;

namespace Raid.Service.DataModel
{
    public class StaticDataCache : IModelDataSource
    {
        private static readonly StaticDataCache s_instance = new StaticDataCache();
        private static readonly StorageSettings s_settings;
        public static StaticDataCache Instance => s_instance;

        private Dictionary<string, object> m_data = new();
        private StaticDataCache()
        { }

        public T Get<T>(string key) where T : class
        {
            if (!m_data.TryGetValue(key, out object value))
            {
                value = UserData.Instance.ReadStaticData<T>(key);
                m_data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            m_data[key] = value;
            UserData.Instance.WriteStaticData<T>(key, value);
        }
    }
}
