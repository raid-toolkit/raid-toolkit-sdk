using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class Hero
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("type")]
        public HeroType? Type;

        [JsonProperty("name")]
        public string Name = string.Empty;

        [JsonProperty("id")]
        public int Id = -1;

        [JsonProperty("idOrigin")]
        public int? OriginalId;

        [JsonProperty("level")]
        public int Level = 0;

        [JsonProperty("empowerLevel")]
        public int EmpowerLevel = 0;

        [JsonProperty("rank")]
        public string? Rank;

        [JsonProperty("awakenRank")]
        public string? AwakenRank;

        [JsonProperty("blessing")]
        public string? Blessing;

        [JsonProperty("blessingResetUsed")]
        public bool? FreeBlessingResetUsed = null;

        [JsonProperty("exp")]
        public int Experience;

        [JsonProperty("fullExp")]
        public int FullExperience;

        [JsonProperty("deleted")]
        public bool Deleted;

        [JsonProperty("locked")]
        public bool Locked = false;

        [JsonProperty("inVault")]
        public bool InVault = false;

        [JsonProperty("inDeepVault")]
        public bool InDeepVault = false;

        [JsonProperty("marker")]
        public string? Marker;

        [JsonProperty("masteries")]
        public MasteryKindId[]? Masteries;

        [JsonProperty("assignedMasteryScrolls")]
        public IReadOnlyDictionary<string, int>? AssignedMasteryScrolls;

        [JsonProperty("unassignedMasteryScrolls")]
        public IReadOnlyDictionary<string, int>? UnassignedMasteryScrolls;

        [JsonProperty("totalMasteryScrolls")]
        public IReadOnlyDictionary<string, int>? TotalMasteryScrolls;

        [JsonProperty("equippedArtifactIds")]
        public IReadOnlyDictionary<string, int>? EquippedArtifactIds;

        [JsonProperty("skillLevelsByTypeId")]
        [Obsolete("Use Skills instead")]
        public IReadOnlyDictionary<int, int>? SkillLevelsByTypeId;

        [JsonProperty("SkillsById")]
        public IReadOnlyDictionary<int, Skill>? SkillsById;
    }
}
