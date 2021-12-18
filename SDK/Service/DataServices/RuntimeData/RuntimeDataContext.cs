using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class RuntimeDataContext : IDataContext
    {
        public static readonly RuntimeDataContext Default = new();

        public RuntimeDataContext()
        {
            Parts = new string[] { "runtime" };
        }
        public string[] Parts { get; }

        public override string ToString()
        {
            return "runtime";
        }
    }
}
