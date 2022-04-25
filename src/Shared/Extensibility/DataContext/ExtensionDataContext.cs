namespace Raid.Toolkit.Extensibility
{
    public class ExtensionDataContext : IDataContext
    {
        internal ExtensionDataContext(string extensionId)
        {
            ExtensionId = extensionId;
            Parts = new string[] { "extensions", extensionId };
        }
        public string[] Parts { get; }
        public string ExtensionId { get; }

        public override string ToString()
        {
            return $"account:{ExtensionId.Substring(0, 16)}";
        }
    }
}
