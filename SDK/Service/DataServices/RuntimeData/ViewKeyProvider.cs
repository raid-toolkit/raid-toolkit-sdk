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
            var currentNote = scope.RaidApplication?._performanceMonitor?._currentNote;
            return PrimaryProvider.Write(context, new ViewKeyDataObject
            {
                ViewId = currentNote?.ViewId,
                ViewKey = currentNote?.ViewKey,
            });
        }
    }
}
