using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class AccountDataContext : IDataContext
    {
        public AccountDataContext(string accountId)
        {
            Parts = new string[] { "accounts", accountId };
        }
        public string[] Parts { get; }
    }
}
