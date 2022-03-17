using System;
using System.Reflection;
using System.Threading.Tasks;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class RealtimeApi : ApiBase<IRealtimeApi>, IRealtimeApi
    {
        internal RealtimeApi(RaidToolkitClient client) : base(client) { }

#pragma warning disable 0067
        public event EventHandler<SerializableEventArgs> AccountListUpdated;
        public event EventHandler<SerializableEventArgs> ReceiveBattleResponse;
        public event EventHandler<SerializableEventArgs> ViewChanged;
#pragma warning restore 0067

        public Task<Account[]> GetConnectedAccounts()
        {
            return CallMethod<Account[]>(MethodBase.GetCurrentMethod());
        }

        public Task<ViewInfo> GetCurrentViewInfo(string accountId)
        {
            return CallMethod<ViewInfo>(MethodBase.GetCurrentMethod(), accountId);
        }

        public Task<LastBattleDataObject> GetLastBattleResponse(string accountId)
        {
            return CallMethod<LastBattleDataObject>(MethodBase.GetCurrentMethod(), accountId);
        }
    }
}
