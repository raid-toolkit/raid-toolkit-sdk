using System.Collections.Generic;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [PublicApi("static-data")]
    internal class StaticDataApi : ApiHandler
    {
        [PublicApi("getAllData")]
        public StaticData GetAllData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance);
        }

        [PublicApi("getLocalizedStrings")]
        public IReadOnlyDictionary<string, string> GetLocalizedStrings()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).LocalizedStrings;
        }

        [PublicApi("getArenaData")]
        public StaticArenaData GetArenaData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).ArenaData;
        }

        [PublicApi("getArtifactData")]
        public StaticArtifactData GetArtifactData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).ArtifactData;
        }

        [PublicApi("getHeroData")]
        public StaticHeroData GetHeroData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).HeroData;
        }

        [PublicApi("getSkillData")]
        public StaticSkillData GetSkillData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).SkillData;
        }

        [PublicApi("getStageData")]
        public StaticStageData GetStageData()
        {
            return StaticDataFacet.ReadValue(StaticDataCache.Instance).StageData;
        }
    }
}