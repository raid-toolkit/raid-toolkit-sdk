using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class StaticDataContext : IDataContext
    {
        private StaticDataContext() { }
        private static readonly string[] kParts = new string[] { "staticData" };
        public static readonly StaticDataContext Default = new();
        public string[] Parts => kParts;
    }
}
