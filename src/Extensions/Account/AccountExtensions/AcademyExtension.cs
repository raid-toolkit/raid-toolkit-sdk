using Client.Model.Gameplay.Artifacts;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;

using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class AcademyExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<IGetAccountDataApi<AcademyData>>,
    IGetAccountDataApi<AcademyData>,
    IAccountExportable
{
    private const string Key = "academy.json";

    IGetAccountDataApi<AcademyData> IAccountPublicApi<IGetAccountDataApi<AcademyData>>.GetApi() => this;
    bool IGetAccountDataApi<AcademyData>.TryGetData(out AcademyData data) => Storage.TryRead(Key, out data);

    public AcademyExtension(IAccount account, IExtensionStorage storage, ILogger<AcademyExtension> logger)
    : base(account, storage, logger)
    {
    }

    protected override Task Update(ModelScope scope)
    {
        if (!Account.TryGetApi<IGetAccountDataApi<StaticAcademyData>>(out var api)
            || !api.TryGetData(out StaticAcademyData academyBonuses))
            return Task.CompletedTask;

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
                            StatBonuses = academyBonuses.GuardianBonusByRarity[(HeroRarity)rarityPair.Key].Take(assignedHeroes.Length).SelectMany(bonuses => bonuses).ToArray(),
                            AssignedHeroes = assignedHeroes.Select(slot => new GuardiansSlot()
                            {
                                FirstHero = slot.FirstHero,
                                SecondHero = slot.SecondHero
                            }).ToArray()
                        };
                    }
                )
            );

        Storage.Write(Key, new AcademyData
        {
            Guardians = guardians
        });
        return Task.CompletedTask;
    }

    public void Export(IAccountReaderWriter account)
    {
        if (Storage.TryRead(Key, out AcademyData data))
            account.Write(Key, data);
    }

    public void Import(IAccountReaderWriter account)
    {
        if (account.TryRead(Key, out AcademyData? data))
            Storage.Write(Key, data);
    }
}
