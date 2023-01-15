using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class Artifact
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("kindId")]
        public string? KindId;

        [JsonProperty("setKindId")]
        public string? SetKindId;

        [JsonProperty("rank")]
        public string? Rank;

        [JsonProperty("rarity")]
        public string? RarityId;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("ascendLevel")]
        public int? AscendLevel;

        [JsonProperty("faction")]
        public string? Faction;

        [JsonProperty("seen")]
        public bool Seen;

        [JsonProperty("activated")]
        public bool Activated;

        [JsonProperty("sellPrice")]
        public int SellPrice;

        [JsonProperty("price")]
        public int Price;

        [JsonProperty("failedUpgrades")]
        public int FailedUpgrades;

        [JsonProperty("primaryBonus")]
        public ArtifactStatBonus? PrimaryBonus;

        [JsonProperty("secondaryBonuses")]
        public IReadOnlyList<ArtifactStatBonus>? SecondaryBonuses;

        [JsonProperty("ascendBonus")]
        public ArtifactStatBonus? AscendBonus;

        [JsonProperty("revision")]
        public int Revision;
    }
}
