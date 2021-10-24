using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [PublicApi("static-data")]
    internal class StaticDataApi : ApiHandler
    {
        private StaticDataCache StaticDataCache;
        public StaticDataApi(ILogger<ApiHandler> logger, StaticDataCache staticData)
            : base(logger) => StaticDataCache = staticData;

        [PublicApi("getAllData")]
        public StaticData GetAllData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache);
        }

        [PublicApi("getLocalizedStrings")]
        public IReadOnlyDictionary<string, string> GetLocalizedStrings()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).LocalizedStrings;
        }

        [PublicApi("getArenaData")]
        public StaticArenaData GetArenaData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).ArenaData;
        }

        [PublicApi("getArtifactData")]
        public StaticArtifactData GetArtifactData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).ArtifactData;
        }

        [PublicApi("getHeroData")]
        public StaticHeroData GetHeroData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).HeroData;
        }

        [PublicApi("getSkillData")]
        public StaticSkillData GetSkillData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).SkillData;
        }

        [PublicApi("getStageData")]
        public StaticStageData GetStageData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache).StageData;
        }
    }
}