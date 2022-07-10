using Client.Model;
using Client.RaidApp;
using Il2CppToolkit.Runtime;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Realtime
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
                    _AppModel = Client.App.SingleInstance<AppModel>._instance.GetValue(Context);
                return _AppModel;
            }
        }

        private RaidApplication _RaidApplication;
        public RaidApplication RaidApplication
        {
            get
            {
                if (_RaidApplication == null)
                    _RaidApplication = Client.App.Application._instance.GetValue(Context) as RaidApplication;
                return _RaidApplication;
            }
        }

        public ModelScope(Il2CsRuntimeContext context)
        {
            Context = context;
        }
    }
}
