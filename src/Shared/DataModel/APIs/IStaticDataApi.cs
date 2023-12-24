using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel
{
    [DeprecatedInV3]
    [PublicApi("static-data")]
    public interface IStaticDataApi
    {
        [PublicApi("getAllData")]
        Task<StaticData> GetAllData();

        [PublicApi("getLocalizedStrings")]
        Task<IReadOnlyDictionary<string, string>> GetLocalizedStrings();

        [PublicApi("getArenaData")]
        Task<StaticArenaData> GetArenaData();

        [PublicApi("getArtifactData")]
        Task<StaticArtifactData> GetArtifactData();

        [PublicApi("getHeroData")]
        Task<StaticHeroTypeData> GetHeroData();

        [PublicApi("getSkillData")]
        Task<StaticSkillData> GetSkillData();

        [PublicApi("getStageData")]
        Task<StaticStageData> GetStageData();
    }
}
