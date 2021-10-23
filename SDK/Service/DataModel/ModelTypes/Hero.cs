using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Heroes;
using SharedModel.Meta.Masteries;

namespace Raid.Service.DataModel
{
    public class Hero
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("type")]
        public HeroType Type;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("idOrigin")]
        public int OriginalId;

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
        public MasteryKindId[] Masteries;

        [JsonProperty("assignedMasteryScrolls")]
        public NumericDictionary<MasteryPointType, int> AssignedMasteryScrolls;

        [JsonProperty("unassignedMasteryScrolls")]
        public NumericDictionary<MasteryPointType, int> UnassignedMasteryScrolls;

        [JsonProperty("totalMasteryScrolls")]
        public NumericDictionary<MasteryPointType, int> TotalMasteryScrolls;

        [JsonProperty("equippedArtifactIds")]
        public NumericDictionary<ArtifactKindId, int> EquippedArtifactIds;

        [JsonProperty("skillLevelsByTypeId")]
        [Obsolete("Use Skills instead")]
        public IReadOnlyDictionary<int, int> SkillLevelsByTypeId;

        [JsonProperty("SkillsById")]
        public IReadOnlyDictionary<int, Skill> SkillsById;
    }
}