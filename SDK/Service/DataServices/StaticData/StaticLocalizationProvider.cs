using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("localization")]
    public class StaticLocalizationDataObject : StaticLocalizationData
    {
    }

    public class StaticLocalizationProvider : DataProviderBase<StaticDataContext, StaticLocalizationDataObject>
    {
        public StaticLocalizationProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticLocalizationDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticLocalizationDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var localizedStrings = new Dictionary<string, string>(staticData.ClientLocalization.Concat(staticData.StaticDataLocalization)
                .GroupBy(x => x.Key)
                .Select(g => g.First()));
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                LocalizedStrings = localizedStrings
            });
        }
    }
}
