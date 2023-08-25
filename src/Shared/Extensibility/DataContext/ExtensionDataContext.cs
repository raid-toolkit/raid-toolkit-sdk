using System.Linq;

namespace Raid.Toolkit.Extensibility
{
    public class ExtensionDataContext : IDataContext
    {
        public ExtensionDataContext(string extensionId)
        {
            ExtensionId = extensionId;
            Parts = new string[] { "extensions", extensionId };
        }

        public ExtensionDataContext(AccountDataContext account, string extensionId)
            : this(extensionId)
        {
            Parts = account.Parts.Concat(Parts).ToArray();
        }

        public string[] Parts { get; }
        public string ExtensionId { get; }
        public AccountDataContext? Account { get; }

        public override string ToString()
        {
            if (Account == null)
            {
                return $"extension:{ExtensionId}";
            }
            else
            {
                return $"{Account};extension:{ExtensionId}";
            }
        }
    }
}
