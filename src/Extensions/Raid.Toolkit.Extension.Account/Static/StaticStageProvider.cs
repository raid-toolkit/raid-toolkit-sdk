using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;
using System.Collections.Generic;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticStageProvider : DataProvider<StaticDataContext, StaticStageData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "stages";
        public override Version Version => kVersion;

        public StaticStageProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            ModelScope scope = new(runtime);
            var hash = scope.StaticDataManager._hash;
            if (Storage.TryRead(context, Key, out StaticStageData previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var areas = new Dictionary<SharedModel.Meta.Stages.AreaTypeId, AreaData>();
            var regions = new Dictionary<SharedModel.Meta.Stages.RegionTypeId, RegionData>();
            Dictionary<int, StageData> stages = new();
            foreach (var area in staticData.StageData.Areas)
            {
                areas.Add(area.Id, area.ToModel());
                foreach (var region in area.Regions)
                {
                    regions.Add(region.Id, region.ToModel(area.Id));
                    foreach (var stagesList in region.StagesByDifficulty.Values)
                    {
                        foreach (var entry in stagesList.ToDictionary(stage => stage.Id, stage => stage.ToModel(area.Id, region.Id)))
                        {
                            stages.Add(entry.Key, entry.Value);
                        }
                    }
                }
            }
            return Storage.Write(context, Key, new StaticStageData
            {
                Hash = hash,
                Areas = areas.ToModel(),
                Regions = regions.ToModel(),
                Stages = stages
            });
        }
    }
}
