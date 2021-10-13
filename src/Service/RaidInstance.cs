using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public class RaidInstance
    {
        static List<Type> s_facetTypes;
        static RaidInstance()
        {
            s_facetTypes = typeof(RaidInstance).Assembly.GetTypesAssignableTo<IFacet>().ToList();
        }
        public static IEnumerable<RaidInstance> Instances { get { return m_instances.Values.ToArray(); } }
        private static ConcurrentDictionary<int, RaidInstance> m_instances = new();
        private Process m_process;
        private readonly Il2CsRuntimeContext m_runtime;
        private readonly Dictionary<IFacet, object> m_facets;

        public RaidInstance(Process process)
        {
            m_process = process;
            m_runtime = new Il2CsRuntimeContext(process);
            m_instances.TryAdd(process.Id, this);
            m_process.Disposed += HandleProcessDisposed;
            m_facets = new(s_facetTypes.Select(type => new KeyValuePair<IFacet, object>((IFacet)Activator.CreateInstance(type), null)));
        }

        private void HandleProcessDisposed(object sender, EventArgs e)
        {
            m_runtime.Dispose();
            m_instances.TryRemove(new(m_process.Id, this));
        }

        public void Update()
        {
            ModelScope scope = new(m_runtime);
            foreach ((IFacet facet, object currentValue) in m_facets)
            {
                object newValue = facet.Merge(scope, currentValue);
                m_facets[facet] = newValue;
            }
        }

        public T GetFacetValue<T>(string id)
        {
            return (T)m_facets.First(facet => facet.Key.Id == id).Value;
        }

        [Size(16)]
        private struct AppModelStaticFields
        {
            [Offset(8)]
#pragma warning disable 649
            public Client.Model.AppModel Instance;
#pragma warning restore 649
        }
    }
}