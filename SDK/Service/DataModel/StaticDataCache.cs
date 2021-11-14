using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppToolkit.Runtime;

namespace Raid.Service
{
    public class StaticDataCache : IModelDataSource
    {
        private readonly Dictionary<string, object> Data = new();
        private readonly Dictionary<IStaticFacet, bool> FacetState;
        private readonly UserData UserData;
        public bool IsReady { get; private set; }

        public StaticDataCache(UserData userData)
        {
            UserData = userData;
            FacetState = typeof(RaidInstance).Assembly.GetTypesAssignableTo<IStaticFacet>()
                .ToDictionary(type => (IStaticFacet)Activator.CreateInstance(type), _ => false);

            foreach (IStaticFacet facet in FacetState.Keys)
            {
                facet.GetValue(this);
            }
            IsReady = !Data.Values.Contains(null);
            if (IsReady)
                LastUpdated = DateTime.UtcNow;
        }

        public DateTime LastUpdated { get; private set; }

        public void Update(Il2CsRuntimeContext runtime)
        {
            ModelScope scope = new(runtime);
            foreach ((IStaticFacet facet, bool successfulRead) in FacetState)
            {
                if (successfulRead)
                {
                    continue;
                }
                string name = FacetAttribute.GetName(facet.GetType());
                Data.TryGetValue(name, out object currentValue);
                object newValue = facet.Merge(scope, currentValue);
                if (newValue != null)
                {
                    FacetState[facet] = true;
                }
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

        public T Get<T>(string key) where T : class
        {
            if (!Data.TryGetValue(key, out object value))
            {
                value = UserData.ReadStaticData<T>(key);
                Data.Add(key, value);
            }
            return (T)value;
        }

        public void Set<T>(string key, T value) where T : class
        {
            Data[key] = value;
            UserData.WriteStaticData<T>(key, value);
        }
    }
}
