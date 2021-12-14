using Il2CppToolkit.Runtime;
using Raid.Service.DataServices;

namespace Raid.Service
{
    public class Il2CsRuntimeFactory
    {
        private readonly RaidInstanceFactory InstanceFactory;

        public Il2CsRuntimeFactory(RaidInstanceFactory instanceFactory)
        {
            InstanceFactory = instanceFactory;
        }

        public Il2CsRuntimeContext GetRuntime(AccountDataContext context)
        {
            return InstanceFactory.GetById(context.AccountId).Runtime;
        }
    }
}
