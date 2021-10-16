using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Artifacts.Sets;
using SharedModel.Meta.Heroes;

namespace Raid.Service.DataModel
{
    public class Artifact
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("kindId")]
        public ArtifactKindId KindId;

        [JsonProperty("setKindId")]
        public ArtifactSetKindId SetKindId;

        [JsonProperty("rank")]
        public ArtifactRankId Rank;

        [JsonProperty("rarity")]
        public ArtifactRarityId RarityId;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("faction")]
        public HeroFraction Faction;

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
        public ArtifactStatBonus PrimaryBonus;

        [JsonProperty("secondaryBonuses")]
        public IReadOnlyList<ArtifactStatBonus> SecondaryBonuses;

        [JsonProperty("revision")]
        public int Revision;
    }
}