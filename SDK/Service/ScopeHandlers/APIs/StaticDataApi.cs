using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using Raid.Service.DataServices;

namespace Raid.Service
{
    [PublicApi("static-data")]
    internal class StaticDataApi : ApiHandler<StaticDataApi>
    {
        private readonly StaticDataCache StaticDataCache;
        private readonly StaticHeroTypeProvider HeroTypeProvider;
        public StaticDataApi(ILogger<StaticDataApi> logger, StaticDataCache staticData, StaticHeroTypeProvider heroTypeProvider)
            : base(logger)
        {
            StaticDataCache = staticData;
            HeroTypeProvider = heroTypeProvider;
        }

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
        public StaticHeroTypeData GetHeroData()
        {
            return HeroTypeProvider.GetValue(StaticDataContext.Default);
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
