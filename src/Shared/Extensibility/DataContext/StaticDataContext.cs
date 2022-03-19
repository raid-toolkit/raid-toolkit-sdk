namespace Raid.Toolkit.Extensibility
{
    public class StaticDataContext : IDataContext
    {
        public static readonly StaticDataContext Default = new();

        private StaticDataContext() { }
        private static readonly string[] kParts = new string[] { "static" };
        public string[] Parts => kParts;

        public override string ToString()
        {
            return "static";
        }
    }
}
