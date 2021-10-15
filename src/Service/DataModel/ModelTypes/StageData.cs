using Newtonsoft.Json;
using SharedModel.Meta.Stages;

namespace Raid.Service.DataModel
{
    public class AreaData : NamedValue
    {
        [JsonProperty("areaId")]
        public AreaTypeId AreaId;
    }

    public class RegionData : AreaData
    {
        [JsonProperty("regionId")]
        public RegionTypeId RegionId;
    }

    public class StageData : RegionData
    {
        [JsonProperty("stageId")]
        public int StageId;

        [JsonProperty("difficulty")]
        public DifficultyId? Difficulty;

        [JsonProperty("bossName")]
        public LocalizedText BossName;
    }

    public static partial class ModelExtensions
    {
        public static AreaData ToModel(this SharedModel.Meta.Stages.Area area)
        {
            return new()
            {
                Name = area.Name.ToModel(),
                AreaId = area.Id
            };
        }
        public static RegionData ToModel(this SharedModel.Meta.Stages.Region region, AreaTypeId areaTypeId)
        {
            return new()
            {
                Name = region.Name.ToModel(),
                RegionId = region.Id,
                AreaId = areaTypeId
            };
        }
        public static StageData ToModel(this SharedModel.Meta.Stages.Stage stage, AreaTypeId areaTypeId, RegionTypeId regionTypeId)
        {
            return new()
            {
                Name = stage.Name.ToModel(),
                AreaId = areaTypeId,
                RegionId = regionTypeId,
                Difficulty = stage._difficulty.ToNullable(),
                StageId = stage.Id,
            };
        }
    }
}