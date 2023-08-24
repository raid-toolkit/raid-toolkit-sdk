using System.Collections.Generic;
using Newtonsoft.Json;
using Raid.Toolkit.DataModel.Enums;

namespace Raid.Toolkit.DataModel
{
    public class AcademyData
    {
        [JsonProperty("guardians")]
        public Dictionary<HeroFraction, Dictionary<HeroRarity, GuardianData>> Guardians;
    }

    public class GuardianData
    {
        [JsonProperty("bonuses")]
        public StatBonus[] StatBonuses;

        [JsonProperty("assignedHeroes")]
        public GuardiansSlot[] AssignedHeroes;
    }

    public class GuardiansSlot
    {
        [JsonProperty("firstHero")]
        public int? FirstHero;

        [JsonProperty("secondHero")]
        public int? SecondHero;
    }
}
