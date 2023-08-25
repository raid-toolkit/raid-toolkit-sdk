using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extension.Account
{
    [PublicApi("static-data")]
    internal class StaticDataApi : ApiHandler<IStaticDataApi>, IStaticDataApi, IDisposable
    {
        private readonly StaticDataWrapper StaticData;
        public StaticDataApi(
            IExtensionHost host,
            ILogger<StaticDataApi> logger)
            : base(logger)
        {
            StaticData = new(host);
        }

        [PublicApi("getAllData")]
        public Task<StaticData> GetAllData()
        {
            return Task.FromResult(new StaticData()
            {
                ArenaData = StaticData.Arena,
                ArtifactData = StaticData.Artifacts,
                HeroData = StaticData.HeroTypes,
                LocalizedStrings = StaticData.Localization.LocalizedStrings,
                SkillData = StaticData.Skills,
                StageData = StaticData.Stages
            });
        }

        [PublicApi("getLocalizedStrings")]
        public Task<IReadOnlyDictionary<string, string>> GetLocalizedStrings()
        {
            return Task.FromResult(StaticData.Localization.LocalizedStrings);
        }

        [PublicApi("getArenaData")]
        public Task<StaticArenaData> GetArenaData()
        {
            return Task.FromResult(StaticData.Arena);
        }

        [PublicApi("getArtifactData")]
        public Task<StaticArtifactData> GetArtifactData()
        {
            return Task.FromResult(StaticData.Artifacts);
        }

        [PublicApi("getHeroData")]
        public Task<StaticHeroTypeData> GetHeroData()
        {
            return Task.FromResult(StaticData.HeroTypes);
        }

        [PublicApi("getSkillData")]
        public Task<StaticSkillData> GetSkillData()
        {
            return Task.FromResult(StaticData.Skills);
        }

        [PublicApi("getStageData")]
        public Task<StaticStageData> GetStageData()
        {
            return Task.FromResult(StaticData.Stages);
        }

        public void Dispose() { }
    }
}
