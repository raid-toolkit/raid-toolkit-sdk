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
        public static IEnumerable<RaidInstance> Instances { get { return s_instances.Values.ToArray(); } }
        private static ConcurrentDictionary<int, RaidInstance> s_instances = new();

        public static RaidInstance GetById(string id)
        {
            return s_instances.Single(instance => instance.Value.m_id == id).Value;
        }

        private Process m_process;
        private readonly Il2CsRuntimeContext m_runtime;
        private readonly Dictionary<IFacet, object> m_facets;
        private readonly string m_id;
        private readonly UserAccount m_userAccount;

        public RaidInstance(Process process)
        {
            m_process = process;
            m_process.Disposed += HandleProcessDisposed;

            m_runtime = new Il2CsRuntimeContext(process);
            m_id = GetAccountId();
            m_userAccount = UserData.Instance.GetAccount(m_id);
            IFacet currentFacet = null;
            m_facets = s_facetTypes.ToDictionary(type => (currentFacet = (IFacet)Activator.CreateInstance(type)), _ => currentFacet.GetValue(m_userAccount));

            s_instances.TryAdd(process.Id, this);
        }

        private void HandleProcessDisposed(object sender, EventArgs e)
        {
            m_runtime.Dispose();
            s_instances.TryRemove(new(m_process.Id, this));
        }

        public void Update()
        {
            ModelScope scope = new(m_runtime);
            foreach ((IFacet facet, object currentValue) in m_facets)
            {
                object newValue = facet.Merge(scope, currentValue);
                m_facets[facet] = newValue;
                if (newValue != currentValue)
                {
                    m_userAccount.Set(FacetAttribute.GetName(facet.GetType()), newValue);
                }
            }
        }

        private string GetAccountId()
        {
            ModelScope scope = new(m_runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return string.Join('_', globalId, socialId).Sha256();
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