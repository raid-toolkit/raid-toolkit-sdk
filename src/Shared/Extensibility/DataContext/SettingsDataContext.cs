namespace Raid.Toolkit.Extensibility
{
    public class SettingsDataContext : IDataContext
    {
        public static readonly SettingsDataContext Default = new();

        private SettingsDataContext() { }
        private static readonly string[] kParts = new string[] { "." };
        public string[] Parts => kParts;

        public override string ToString()
        {
            return ".";
        }
    }
}
