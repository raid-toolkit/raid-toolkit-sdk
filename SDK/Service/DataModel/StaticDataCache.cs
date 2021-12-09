using System;
using System.Collections.Generic;
using System.Linq;

namespace Raid.Service
{
    public class StaticDataCache : IModelDataSource
    {
        private readonly Dictionary<string, object> Data = new();
        private readonly List<IStaticFacet> Facets;
        private readonly AppData UserData;
        public bool IsReady { get; private set; }

        public StaticDataCache(AppData userData)
        {
            UserData = userData;
            Facets = typeof(RaidInstance).Assembly.GetTypesAssignableTo<IStaticFacet>()
                .Select(type => (IStaticFacet)Activator.CreateInstance(type))
                .ToList();

            foreach (IStaticFacet facet in Facets)
            {
                _ = facet.GetValue(this);
            }
            IsReady = !Data.Values.Contains(null);
            if (IsReady)
                LastUpdated = DateTime.UtcNow;
        }

        public DateTime LastUpdated { get; private set; }

        public void Update(ModelScope scope)
        {
            foreach (IStaticFacet facet in Facets)
            {
                string name = FacetAttribute.GetName(facet.GetType());
                _ = Data.TryGetValue(name, out object currentValue);
                object newValue = facet.Merge(scope, currentValue);
                if (newValue != currentValue)
                {
                    Set(name, newValue);
                }
            }

            if (!IsReady)
            {
                IsReady = !Data.Values.Contains(null);
            }

            if (IsReady)
                LastUpdated = DateTime.UtcNow;
        }

        public T Read<T>(string key) where T : class
        {
            return UserData.ReadStaticData<T>(key);
        }

        public T Get<T>(string key) where T : class
        {
            if (!Data.TryGetValue(key, out object value))
            {
                value = Read<T>(key);
                Data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            Data[key] = value;
            UserData.WriteStaticData(key, value);
        }
    }
}
