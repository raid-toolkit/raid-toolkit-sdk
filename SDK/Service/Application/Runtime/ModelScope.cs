using Client.Model;
using Client.Model.Gameplay.StaticData;
using Client.RaidApp;
using Client.ViewModel;
using Il2CppToolkit.Runtime;

namespace Raid.Service
{
    public class ModelScope
    {
        public Il2CsRuntimeContext Context { get; }

        private AppModel _AppModel;
        public AppModel AppModel
        {
            get
            {
                if (_AppModel == null)
                    _AppModel = Client.App.SingleInstance<AppModel>.method_get_Instance
                        .GetMethodInfo(Context).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppModel>>().Instance;
                return _AppModel;
            }
        }

        private AppViewModel _AppViewModel;
        public AppViewModel AppViewModel
        {
            get
            {
                if (_AppViewModel == null)
                    _AppViewModel = Client.App.SingleInstance<AppViewModel>.method_get_Instance
                        .GetMethodInfo(Context).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppViewModel>>().Instance;
                return _AppViewModel;
            }
        }

        private ClientStaticDataManager _StaticDataManager;
        public ClientStaticDataManager StaticDataManager
        {
            get
            {
                if (_StaticDataManager == null)
                    _StaticDataManager = AppModel.StaticDataManager as ClientStaticDataManager;
                return _StaticDataManager;
            }
        }

        private Contexts _Contexts;
        public Contexts Contexts
        {
            get
            {
                if (_Contexts == null)
                    _Contexts = Contexts.GetStaticFields(Context)._sharedInstance;
                return _Contexts;
            }
        }

        private RaidApplication _RaidApplication;
        public RaidApplication RaidApplication
        {
            get
            {
                if (_RaidApplication == null)
                    _RaidApplication = Client.App.Application.GetStaticFields(Context)._instance as RaidApplication;
                return _RaidApplication;
            }
        }

        public ModelScope(Il2CsRuntimeContext context)
        {
            Context = context;
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
