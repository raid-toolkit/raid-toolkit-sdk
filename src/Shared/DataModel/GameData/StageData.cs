using Newtonsoft.Json;

using Raid.Toolkit.DataModel.Enums;

using System;
using System.Collections.Generic;

namespace Raid.Toolkit.DataModel
{
    public class AreaData : NamedValue
    {
        [JsonProperty("areaId")]
        public int AreaId;
    }

    public class RegionData : AreaData
    {
        [JsonProperty("regionId")]
        public int RegionId;
    }

    public class StageData : RegionData
    {
        [JsonProperty("stageId")]
        public int StageId;

        [JsonProperty("difficulty")]
        public string Difficulty;

        [JsonProperty("bossName")]
        public LocalizedText BossName;

        [JsonProperty("modifiers")]
        public StatsModifier[] Modifiers;

        [JsonProperty(propertyName: "formations")]
        public StageFormation[] Formations = Array.Empty<StageFormation>();
    }

    public class StatsModifier : StatBonus
    {
        [JsonProperty("round")]
        public int Round;

        [JsonProperty("bossOnly")]
        public bool BossOnly;
    }

    public class StatsModifierSetup
    {
        [JsonProperty(propertyName: "percBonus")]
        public Stats? PercentBonus;

        [JsonProperty(propertyName: "flatBonus")]
        public Stats? FlatBonus;

        [JsonProperty(propertyName: "initialState")]
        public StatsInitialState? InitialState;
    }

    public class StatsInitialState
    {
        [JsonProperty(propertyName: "hpModifier")]
        public double HealthModifier;

        [JsonProperty(propertyName: "damageTaken")]
        public double DamageTaken;
    }

    public class StageFormation
    {
        [JsonProperty(propertyName: "id")]
        public int Id;

        [JsonProperty(propertyName: "heroSetups")]
        public HeroSlotSetup[] HeroSetups = Array.Empty<HeroSlotSetup>();
    }

    public class HeroSlotSetup
    {
        [JsonProperty(propertyName: "round")]
        public int Round;

        [JsonProperty(propertyName: "slot")]
        public int Slot;

        [JsonProperty(propertyName: "typeId")]
        public int TypeId;

        [JsonProperty(propertyName: "grade")]
        public HeroGrade Grade;

        [JsonProperty(propertyName: "level")]
        public int Level;

        [JsonProperty(propertyName: "awaken")]
        public int Awaken;

        [JsonProperty(propertyName: "maxSkillsLevel")]
        public bool MaxSkillsLevel;

        [JsonProperty(propertyName: "modifiers")]
        public StatsModifierSetup? Modifiers;
    }
}
