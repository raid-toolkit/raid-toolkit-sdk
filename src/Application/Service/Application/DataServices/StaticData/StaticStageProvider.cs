using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("stages")]
    public class StaticStageDataObject : StaticStageData
    {
    }

    public class StaticStageProvider : DataProviderBase<StaticDataContext, StaticStageDataObject>
    {
        public StaticStageProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticStageDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticStageDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var areas = new Dictionary<SharedModel.Meta.Stages.AreaTypeId, AreaData>();
            var regions = new Dictionary<SharedModel.Meta.Stages.RegionTypeId, RegionData>();
            var stages = new List<KeyValuePair<int, StageData>>();
            foreach (var area in staticData.StageData.Areas)
            {
                areas.Add(area.Id, area.ToModel());
                foreach (var region in area.Regions)
                {
                    regions.Add(region.Id, region.ToModel(area.Id));
                    foreach (var stagesList in region.StagesByDifficulty.Values)
                    {
                        stages.AddRange(
                            stagesList.ToDictionary(stage => stage.Id, stage => stage.ToModel(area.Id, region.Id)).AsEnumerable()
                            );
                    }
                }
            }
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                Areas = areas.ToModel(),
                Regions = regions.ToModel(),
                Stages = new Dictionary<int, StageData>(stages)
            });
        }
    }
}
