using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;
using Raid.Toolkit.Extensibility;
using Client.Model;
using Client.Model.Gameplay.StaticData;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticAcademyProvider : DataProvider<StaticDataContext, StaticAcademyData>
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticAcademyProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            var appModel = Client.App.SingleInstance<AppModel>.method_get_Instance
                        .GetMethodInfo(runtime).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppModel>>().Instance;
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
