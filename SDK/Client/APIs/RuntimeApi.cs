using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Raid.DataModel;
using Raid.Service;

namespace Raid.Client
{
    public class RuntimeApi : ApiBase<IRuntimeApi>, IRuntimeApi
    {
        internal RuntimeApi(RaidToolkitClient client) : base(client) { }

        public event EventHandler<SerializableEventArgs> Updated;

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