using Raid.Service.DataServices;

namespace Raid.Service
{
    public class ModelScopeFactory
    {
        private readonly Il2CsRuntimeFactory RuntimeFactory;

        public ModelScopeFactory(Il2CsRuntimeFactory runtimeFactory)
        {
            RuntimeFactory = runtimeFactory;
        }

        public ModelScope GetModelScope(AccountDataContext context)
        {
            return new ModelScope(RuntimeFactory.GetRuntime(context));
        }
    }
}