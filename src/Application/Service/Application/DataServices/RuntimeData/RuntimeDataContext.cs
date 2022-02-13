using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class RuntimeDataContext : IDataContext
    {
        public RuntimeDataContext(string accountId)
        {
            AccountId = accountId;
            Parts = new string[] { "runtime", accountId };
        }

        public string[] Parts { get; }
        public string AccountId { get; }

        public static implicit operator RuntimeDataContext(string accountId)
        {
            return new(accountId);
        }

        public override string ToString()
        {
            return $"runtime:{AccountId[..16]}";
        }
    }
}
