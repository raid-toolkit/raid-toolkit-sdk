using System.Linq;
using Raid.DataModel;

namespace Raid.Service
{
    [Facet("staticAcademyData")]
    public class StaticAcademyDataFacet : StaticFacetBase<StaticAcademyData, StaticAcademyDataFacet>
    {
        protected override StaticAcademyData Merge(ModelScope scope, StaticAcademyData previous = null)
        {
            var hash = scope.StaticDataManager._hash;
            if (previous?.Hash == hash)
            {
                return previous;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var guardiansBonusData = staticData.AcademyData.Guardians.BonusesByHeroRarity.ToDictionary(
                kvp => (DataModel.Enums.HeroRarity)kvp.Key,
                kvp => kvp.Value.Select(bonuses => bonuses.Bonuses.Select(bonus => bonus.Value.ToModel(bonus.Key)).ToArray()).ToArray()
            );
            return new()
            {
                Hash = hash,
                GuardianBonusByRarity = guardiansBonusData,
            };
        }
    }
}
