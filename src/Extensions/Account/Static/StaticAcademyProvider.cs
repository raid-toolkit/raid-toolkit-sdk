using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;
using Client.Model;
using Client.Model.Gameplay.StaticData;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticAcademyProvider : DataProvider<StaticDataContext, StaticAcademyData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "academy";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticAcademyProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            ModelScope scope = new(runtime);
            var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(runtime);
            ClientStaticDataManager staticDataManager = appModel.StaticDataManager as ClientStaticDataManager;
            var hash = staticDataManager._hash;
            if (Storage.TryRead(context, "academy", out StaticAcademyData previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = staticDataManager.StaticData;
            var guardiansBonusData = staticData.AcademyData.Guardians.BonusesByHeroRarity.ToDictionary(
                kvp => (HeroRarity)kvp.Key,
                kvp => kvp.Value.Select(bonuses => bonuses.Bonuses.Select(bonus => bonus.Value.ToModel(bonus.Key)).ToArray()).ToArray()
            );
            return Storage.Write(context, "academy", new StaticAcademyData()
            {
                Hash = hash,
                GuardianBonusByRarity = guardiansBonusData,
            });
        }
    }
}
