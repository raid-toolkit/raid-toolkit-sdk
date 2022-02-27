using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;
using Raid.Toolkit.Extensibility;
using Client.Model;
using Client.Model.Gameplay.StaticData;

namespace Raid.Toolkit.Extension.Account
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

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            var appModel = Client.App.SingleInstance<AppModel>.method_get_Instance
                        .GetMethodInfo(runtime).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppModel>>().Instance;
            ClientStaticDataManager staticDataManager = appModel.StaticDataManager as ClientStaticDataManager;
            var hash = staticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticAcademyDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = staticDataManager.StaticData;
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
