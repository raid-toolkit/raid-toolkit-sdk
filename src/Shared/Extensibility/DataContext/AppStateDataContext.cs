namespace Raid.Toolkit.Extensibility
{
    public class AppStateDataContext : IDataContext
    {
        public static readonly AppStateDataContext Default = new();

        private AppStateDataContext() { }
        private static readonly string[] kParts = new string[] { "appstate" };
        public string[] Parts => kParts;

        public override string ToString()
        {
            return "appstate";
        }
    }
}
