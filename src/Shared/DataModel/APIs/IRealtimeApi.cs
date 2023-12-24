using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.DataModel
{
    [DeprecatedInV3]
    [PublicApi("realtime-api")]
    public interface IRealtimeApi
    {
        [PublicApi("account-list-updated")]
        event EventHandler<SerializableEventArgs> AccountListUpdated;

        [PublicApi("view-changed")]
        event EventHandler<SerializableEventArgs> ViewChanged;

        [PublicApi("last-battle-response-updated")]
        event EventHandler<SerializableEventArgs> ReceiveBattleResponse;

        [PublicApi("getConnectedAccounts")]
        Task<Account[]> GetConnectedAccounts();

        [PublicApi("getLastBattleResponse")]
        Task<LastBattleDataObject> GetLastBattleResponse(string accountId);

        [PublicApi("getCurrentViewInfo")]
        Task<ViewInfo> GetCurrentViewInfo(string accountId);
    }

    [DeprecatedInV3]
    public class ViewInfo
    {
        [JsonProperty("viewId")]
        public int? ViewId;

        [JsonProperty("viewKey")]
        public string ViewKey;
    }

    [DeprecatedInV3]
    public class GivenDamage
    {
        [JsonProperty("demonLord")]
        public long? DemonLord;
        [JsonProperty("hydra")]
        public long? Hydra;
    }

    [DeprecatedInV3]
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

}
