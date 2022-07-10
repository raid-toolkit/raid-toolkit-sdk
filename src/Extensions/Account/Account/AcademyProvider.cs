using System;
using System.Linq;
using Client.Model;
using Il2CppToolkit.Runtime;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extension.Account
{
    public class AcademyProvider : DataProvider<AccountDataContext, AcademyData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "academy";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public AcademyProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            if (!Storage.TryRead(StaticDataContext.Default, Key, out StaticAcademyData academyBonuses))
                return false;

            var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(runtime);
            var academy = appModel._userWrapper.Academy.AcademyData;
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
                                StatBonuses = academyBonuses.GuardianBonusByRarity[(HeroRarity)rarityPair.Key].Take(assignedHeroes.Length).SelectMany(bonuses => bonuses).ToArray(),
                                AssignedHeroes = assignedHeroes.Select(slot => new GuardiansSlot()
                                {
                                    FirstHero = slot.FirstHero.Value,
                                    SecondHero = slot.SecondHero.Value
                                }).ToArray()
                            };
                        }
                    )
                );

            return Storage.Write(context, Key, new AcademyData
            {
                Guardians = guardians
            });
        }
    }
}
