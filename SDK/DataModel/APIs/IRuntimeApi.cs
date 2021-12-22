using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Raid.Service;

namespace Raid.DataModel
{
    [PublicApi("runtime-api")]
    public interface IRuntimeApi
    {
        [PublicApi("updated")]
        event EventHandler<SerializableEventArgs> Updated;

        [PublicApi("getConnectedAccounts")]
        Task<Account[]> GetConnectedAccounts();

        [PublicApi("getLastBattleResponse")]
        Task<JObject> GetLastBattleResponse(string accountId);
    }
}