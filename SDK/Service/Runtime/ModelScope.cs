using Client.Model;
using Client.Model.Gameplay.StaticData;
using Client.ViewModel;
using Il2CppToolkit.Runtime;

namespace Raid.Service
{
    public class ModelScope
    {
        public Il2CsRuntimeContext Context { get; }
        public AppModel AppModel { get; }
        public AppViewModel AppViewModel { get; }
        public ClientStaticDataManager StaticDataManager { get; }
        public ModelScope(Il2CsRuntimeContext context)
        {
            Context = context;
            AppModel = Client.App.SingleInstance<AppModel>.method_get_Instance.GetMethodInfo(Context).DeclaringClass.StaticFields
                .As<SingleInstanceStaticFields<AppModel>>().Instance;
            AppViewModel = Client.App.SingleInstance<AppViewModel>.method_get_Instance.GetMethodInfo(Context).DeclaringClass.StaticFields
                .As<SingleInstanceStaticFields<AppViewModel>>().Instance;

            StaticDataManager = AppModel.StaticDataManager as ClientStaticDataManager;
        }

        [Size(16)]
        private struct SingleInstanceStaticFields<T>
        {
            [Offset(8)]
#pragma warning disable 649
            public T Instance;
#pragma warning restore 649
        }
    }
}
