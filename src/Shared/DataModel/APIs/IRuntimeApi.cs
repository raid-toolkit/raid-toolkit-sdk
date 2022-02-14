using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Raid.Toolkit.DataModel
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
