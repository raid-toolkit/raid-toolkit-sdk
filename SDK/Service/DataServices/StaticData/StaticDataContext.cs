using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class StaticDataContext : IDataContext
    {
        private static readonly string[] kParts = new string[] { "staticData" };
        public string[] Parts => kParts;
    }
}
