using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("heroTypes")]
    public class StaticHeroTypeDataObject : StaticHeroTypeData
    {
    }

    public class StaticHeroTypeProvider : DataProviderBase<StaticDataContext, StaticHeroTypeDataObject>
    {
        public StaticHeroTypeProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticHeroTypeDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticHeroTypeDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var heroTypes = scope.StaticDataManager.StaticData.HeroData.HeroTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());

            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                HeroTypes = heroTypes,
            });
        }
    }
}
