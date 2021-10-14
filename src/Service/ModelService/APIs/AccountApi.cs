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
            return UserData.Instance.UserAccounts.Select(AccountFacet.ReadValue).ToArray();
        }

        [PublicApi("accountInfo")]
        public async Task<Account> GetAccount(string accountId)
        {
            return AccountFacet.ReadValue(UserData.Instance.GetAccount(accountId));
        }
    }
}