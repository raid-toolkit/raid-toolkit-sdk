using Newtonsoft.Json;

namespace Raid.DataModel
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
    }
}