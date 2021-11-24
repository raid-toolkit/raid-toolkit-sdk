using System.Linq;
using Raid.DataModel;
using Raid.DataModel.Enums;

namespace Raid.Service
{
    [Facet("academy", Version = "1.0")]
    public class AcademyFacet : UserAccountFacetBase<AcademyData, AcademyFacet>
    {
        private readonly StaticDataCache StaticDataCache;
        public AcademyFacet(StaticDataCache staticDataCache)
        {
            StaticDataCache = staticDataCache;
        }

        protected override AcademyData Merge(ModelScope scope, AcademyData previous = null)
        {
            var academyBonuses = StaticAcademyDataFacet.ReadValue(StaticDataCache).GuardianBonusByRarity;

            var academy = scope.AppModel._userWrapper.Academy.AcademyData;

            var guardians = academy.Guardians.SlotsByFraction.UnderlyingDictionary
                .ToDictionary(
                    factionPair => (HeroFraction)factionPair.Key,
                    factionPair => factionPair.Value.SlotsByRarity.ToDictionary(
                        rarityPair => (HeroRarity)rarityPair.Key,
                        rarityPair =>
                        {
                            var assignedHeroes = rarityPair.Value.Where(slot => slot.FirstHero.HasValue && slot.SecondHero.HasValue).ToArray();
                            return new GuardianData()
                            {
                                StatBonuses = academyBonuses[(HeroRarity)rarityPair.Key].Take(assignedHeroes.Length).SelectMany(bonuses => bonuses).ToArray(),
                                AssignedHeroes = assignedHeroes.Select(slot => new GuardiansSlot()
                                {
                                    FirstHero = slot.FirstHero.Value,
                                    SecondHero = slot.SecondHero.Value
                                }).ToArray()
                            };
                        }
                    )
                );
            return new AcademyData
            {
                Guardians = guardians
            };
        }
    }
}
