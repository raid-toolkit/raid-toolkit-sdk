using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Heroes;

namespace Raid.Service.DataModel
{
    public class Hero
    {
        [JsonProperty("id")]
        public int Id;

        [JsonProperty("idOrigin")]
        public int OriginalId;

        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("rank")]
        public HeroGrade Rank;

        [JsonProperty("exp")]
        public int Experience;

        [JsonProperty("fullExp")]
        public int FullExperience;

        [JsonProperty("deleted")]
        public bool Deleted;

        [JsonProperty("locked")]
        public bool Locked;

        [JsonProperty("inVault")]
        public bool InVault;

        [JsonProperty("marker")]
        public HeroMarker Marker;

        [JsonProperty("masteries")]
        public IReadOnlyList<int> Masteries;

        [JsonProperty("equippedArtifactIds")]
        public IReadOnlyDictionary<ArtifactKindId, int> EquippedArtifactIds;

        [JsonProperty("skillLevelsByTypeId")]
        [Obsolete("Use Skills instead")]
        public IReadOnlyDictionary<int, int> SkillLevelsByTypeId;

        [JsonProperty("SkillsById")]
        public IReadOnlyDictionary<int, Skill> SkillsById;
    }
}