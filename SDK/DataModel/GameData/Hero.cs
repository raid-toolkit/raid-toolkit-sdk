using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.DataModel
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
        public string Rank;

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
        public string Marker;

        [JsonProperty("masteries")]
        public MasteryKindId[] Masteries;

        [JsonProperty("assignedMasteryScrolls")]
        public IReadOnlyDictionary<string, int> AssignedMasteryScrolls;

        [JsonProperty("unassignedMasteryScrolls")]
        public IReadOnlyDictionary<string, int> UnassignedMasteryScrolls;

        [JsonProperty("totalMasteryScrolls")]
        public IReadOnlyDictionary<string, int> TotalMasteryScrolls;

        [JsonProperty("equippedArtifactIds")]
        public IReadOnlyDictionary<string, int> EquippedArtifactIds;

        [JsonProperty("SkillsById")]
        public IReadOnlyDictionary<int, Skill> SkillsById;
    }
}