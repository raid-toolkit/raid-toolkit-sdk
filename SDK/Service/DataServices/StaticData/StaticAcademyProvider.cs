using System.Linq;
using Raid.DataModel;
using Raid.DataModel.Enums;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("academy")]
    public class StaticAcademyDataObject : StaticAcademyData
    {
    }

    public class StaticAcademyProvider : DataProviderBase<StaticDataContext, StaticAcademyDataObject>
    {
        public StaticAcademyProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticAcademyDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticAcademyDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var guardiansBonusData = staticData.AcademyData.Guardians.BonusesByHeroRarity.ToDictionary(
                kvp => (HeroRarity)kvp.Key,
                kvp => kvp.Value.Select(bonuses => bonuses.Bonuses.Select(bonus => bonus.Value.ToModel(bonus.Key)).ToArray()).ToArray()
            );
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                GuardianBonusByRarity = guardiansBonusData,
            });
        }
    }
}
