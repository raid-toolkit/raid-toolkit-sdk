using Newtonsoft.Json;

namespace Raid.Service.DataModel
{
    public class HeroSnapshot : Hero
    {
        [JsonProperty("skills")]
        public SkillSnapshot[] Skills;

        [JsonProperty("stats")]
        public StatsSnapshot Stats;

        [JsonProperty("teams")]
        public string[] Teams;

        public HeroSnapshot(Hero hero)
        {
            TypeId = hero.TypeId;
            Type = hero.Type;
            Name = hero.Name;
            Id = hero.Id;
            OriginalId = hero.OriginalId;
            Level = hero.Level;
            Rank = hero.Rank;
            Experience = hero.Experience;
            FullExperience = hero.FullExperience;
            Deleted = hero.Deleted;
            Locked = hero.Locked;
            InVault = hero.InVault;
            Marker = hero.Marker;
            Masteries = hero.Masteries;
            EquippedArtifactIds = hero.EquippedArtifactIds;
            SkillLevelsByTypeId = hero.SkillLevelsByTypeId;
            SkillsById = hero.SkillsById;
        }
    }
}