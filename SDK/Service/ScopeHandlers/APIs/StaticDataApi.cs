using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using Raid.Service.DataServices;

namespace Raid.Service
{
    [PublicApi("static-data")]
    internal class StaticDataApi : ApiHandler<StaticDataApi>
    {
        private readonly StaticArenaProvider StaticArenaData;
        private readonly StaticArtifactProvider StaticArtifactData;
        private readonly StaticSkillProvider StaticSkillData;
        private readonly StaticHeroTypeProvider StaticHeroTypeData;
        private readonly StaticLocalizationProvider StaticLocalizationData;
        private readonly StaticStageProvider StaticStageData;
        public StaticDataApi(
            ILogger<StaticDataApi> logger,
            StaticArenaProvider staticArenaData,
            StaticArtifactProvider staticArtifactData,
            StaticSkillProvider staticSkillData,
            StaticHeroTypeProvider staticHeroTypeProvider,
            StaticLocalizationProvider staticLocalizationProvider,
            StaticStageProvider staticStageData)
            : base(logger)
        {
            StaticArenaData = staticArenaData;
            StaticArtifactData = staticArtifactData;
            StaticSkillData = staticSkillData;
            StaticHeroTypeData = staticHeroTypeProvider;
            StaticLocalizationData = staticLocalizationProvider;
            StaticStageData = staticStageData;
        }

        [PublicApi("getAllData")]
        public StaticData GetAllData()
        {
            return new StaticData()
            {
                ArenaData = GetArenaData(),
                ArtifactData = GetArtifactData(),
                HeroData = GetHeroData(),
                LocalizedStrings = GetLocalizedStrings(),
                SkillData = GetSkillData(),
                StageData = GetStageData()
            };
        }

        [PublicApi("getLocalizedStrings")]
        public IReadOnlyDictionary<string, string> GetLocalizedStrings()
        {
            return StaticLocalizationData.GetValue(StaticDataContext.Default).LocalizedStrings;
        }

        [PublicApi("getArenaData")]
        public StaticArenaData GetArenaData()
        {
            return StaticArenaData.GetValue(StaticDataContext.Default);
        }

        [PublicApi("getArtifactData")]
        public StaticArtifactData GetArtifactData()
        {
            return StaticArtifactData.GetValue(StaticDataContext.Default);
        }

        [PublicApi("getHeroData")]
        public StaticHeroTypeData GetHeroData()
        {
            return StaticHeroTypeData.GetValue(StaticDataContext.Default);
        }

        [PublicApi("getSkillData")]
        public StaticSkillData GetSkillData()
        {
            return StaticSkillData.GetValue(StaticDataContext.Default);
        }

        [PublicApi("getStageData")]
        public StaticStageData GetStageData()
        {
            return StaticStageData.GetValue(StaticDataContext.Default);
        }
    }
}
