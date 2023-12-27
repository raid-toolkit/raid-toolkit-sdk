namespace Raid.Toolkit.Extensibility
{
    public class AccountDirectoryContext : IDataContext
    {
        private readonly string[] kParts = new string[] { "accounts" };
        public string[] Parts => kParts;
        public override string ToString()
        {
            return $"accounts";
        }
    }

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

        public override string ToString()
        {
            return $"account:{AccountId[..16]}";
        }
    }
}
