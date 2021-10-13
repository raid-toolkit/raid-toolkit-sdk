using System;
using System.Linq;
using System.Threading.Tasks;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [PublicApi("account-api")]
    internal class AccountApi : ApiHandler
    {
        [PublicApi("updated")]
        public event EventHandler<SerializableEventArgs> Updated;

        [PublicApi("getAccounts")]
        public async Task<Account[]> GetAccounts()
        {
            return RaidInstance.Instances.Select(inst => inst.GetFacetValue<Account>("account")).Where(acct => acct != null).ToArray();
        }
    }
}