using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class AccountDataContext : IDataContext
    {
        public AccountDataContext(string accountId)
        {
            AccountId = accountId;
            Parts = new string[] { "accounts", accountId };
        }
        public string[] Parts { get; }
        public string AccountId { get; }

        public static implicit operator AccountDataContext(string accountId)
        {
            return new(accountId);
        }
    }
}
