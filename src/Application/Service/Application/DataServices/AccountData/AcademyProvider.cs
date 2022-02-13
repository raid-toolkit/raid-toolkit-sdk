using System.Linq;
using Raid.DataModel;
using Raid.DataModel.Enums;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("academy")]
    public class AcademyDataObject : AcademyData
    {
    }

    public class AcademyProvider : DataProviderBase<AccountDataContext, AcademyDataObject>
    {
        private readonly StaticAcademyProvider StaticAcademyProvider;

        public AcademyProvider(
            IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, AcademyDataObject> storage,
            StaticAcademyProvider academyProvider)
            : base(storage)
        {
            StaticAcademyProvider = academyProvider;
        }

        public override bool Update(ModelScope scope, AccountDataContext context)
        {
            var academyBonuses = StaticAcademyProvider.GetValue(StaticDataContext.Default).GuardianBonusByRarity;
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

            return PrimaryProvider.Write(context, new AcademyDataObject
            {
                Guardians = guardians
            });
        }
    }
}
