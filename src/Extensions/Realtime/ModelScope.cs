using Client.RaidApp;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extension.Realtime
{
    public class ModelScope
    {
        public Il2CsRuntimeContext Context { get; }

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
    }
}