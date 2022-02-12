using System.Linq;
using Client.RaidApp;
using Newtonsoft.Json;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("ViewKey")]
    public class ViewKeyDataObject
    {
        [JsonProperty("viewId")]
        public int? ViewId;

        [JsonProperty("viewKey")]
        public string ViewKey;
    }

    public class ViewKeyInfoProvider : DataProviderBase<RuntimeDataContext, ViewKeyDataObject>
    {
        public ViewKeyInfoProvider(IDataResolver<RuntimeDataContext, CachedDataStorage<PersistedDataStorage>, ViewKeyDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, RuntimeDataContext context)
        {
            if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
                return false;
            ViewMeta topView = viewMaster._views.Last();
            return PrimaryProvider.Write(context, new ViewKeyDataObject
            {
                ViewId = (int)topView.Key,
                ViewKey = topView.Key.ToString(),
            });
        }
    }
}
