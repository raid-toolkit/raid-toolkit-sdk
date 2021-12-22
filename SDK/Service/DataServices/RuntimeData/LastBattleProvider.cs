using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class GivenDamage
    {
        [JsonProperty("demonLord")]
        public long? DemonLord;
        [JsonProperty("hydra")]
        public long? Hydra;
    }

    [DataType("LastBattle")]
    public class LastBattleDataObject
    {
        [JsonProperty("battleKindId")]
        public string BattleKindId;

        [JsonProperty("heroesExperience")]
        public int HeroesExperience;

        [JsonProperty("heroesExperienceAdded")]
        public int HeroesExperienceAdded;

        [JsonProperty("turnCount")]
        public int? Turns;

        [JsonProperty("givenDamage")]
        public GivenDamage GivenDamage;

        [JsonProperty("tournamentPointsByStateId")]
        public Dictionary<int, int> TournamentPointsByStateId;

        [JsonProperty("masteryPointsByHeroId")]
        public Dictionary<int, Dictionary<string, int>> MasteryPointsByHeroId;
    }

    public class LastBattleProvider : DataProviderBase<RuntimeDataContext, LastBattleDataObject>
    {
        public LastBattleProvider(IDataResolver<RuntimeDataContext, CachedDataStorage<PersistedDataStorage>, LastBattleDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, RuntimeDataContext context)
        {
            var response = scope.AppModel._userWrapper.Battle.BattleData.LastResponse;
            return PrimaryProvider.Write(context, new()
            {
                BattleKindId = response.BattleKindId.ToString(),
                HeroesExperience = response.HeroesExperience,
                HeroesExperienceAdded = response.HeroesExperienceAdded,
                Turns = response.Turns.ToNullable(),
                TournamentPointsByStateId = response.TournamentPointsByStateId.UnderlyingDictionary,
                GivenDamage = new()
                {
                    DemonLord = response.GivenDamageToAllianceBoss.ToNullable(),
                    Hydra = response.GivenDamageToAllianceHydra.ToNullable(),
                },
                MasteryPointsByHeroId = response.MasteryPointsByHeroId?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (Dictionary<string, int>)kvp.Value.UnderlyingDictionary.ToModel())
            });
        }
    }
}
