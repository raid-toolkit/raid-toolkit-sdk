using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class RuntimeApi : ApiBase<IRuntimeApi>, IRuntimeApi
    {
        internal RuntimeApi(RaidToolkitClient client) : base(client) { }

#pragma warning disable 0067
        public event EventHandler<SerializableEventArgs> Updated;
#pragma warning restore 0067

        public Task<Account[]> GetConnectedAccounts()
        {
            return CallMethod<Account[]>(MethodBase.GetCurrentMethod());
        }

        public Task<JObject> GetLastBattleResponse(string accountId)
        {
            return CallMethod<JObject>(MethodBase.GetCurrentMethod(), accountId);
        }
    }
}
