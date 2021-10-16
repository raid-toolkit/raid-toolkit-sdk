using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppToolkit.Runtime;

namespace Raid.Service.DataModel
{
    public class StaticDataCache : IModelDataSource
    {
        private static readonly StaticDataCache s_instance = new StaticDataCache();
        public static StaticDataCache Instance => s_instance;

        private readonly Dictionary<string, object> m_data = new();
        private readonly Dictionary<IStaticFacet, bool> m_facetState;
        public bool IsReady { get; private set; }

        private StaticDataCache()
        {
            m_facetState = typeof(RaidInstance).Assembly.GetTypesAssignableTo<IStaticFacet>()
                .ToDictionary(type => (IStaticFacet)Activator.CreateInstance(type), _ => false);

            foreach (IStaticFacet facet in m_facetState.Keys)
            {
                facet.GetValue(this);
            }
            IsReady = !m_data.Values.Contains(null);
        }

        public void Update(Il2CsRuntimeContext runtime)
        {
            ModelScope scope = new(runtime);
            foreach ((IStaticFacet facet, bool successfulRead) in m_facetState)
            {
                if (successfulRead)
                {
                    continue;
                }
                string name = FacetAttribute.GetName(facet.GetType());
                m_data.TryGetValue(name, out object currentValue);
                object newValue = facet.Merge(scope, currentValue);
                if (newValue != null)
                {
                    m_facetState[facet] = true;
                }
                if (newValue != currentValue)
                {
                    Set(name, newValue);
                }
            }

            if (!IsReady)
            {
                IsReady = !m_data.Values.Contains(null);
            }
        }

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
