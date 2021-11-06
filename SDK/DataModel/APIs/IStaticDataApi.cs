using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raid.DataModel
{
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
        Task<StaticHeroData> GetHeroData();

        [PublicApi("getSkillData")]
        Task<StaticSkillData> GetSkillData();

        [PublicApi("getStageData")]
        Task<StaticStageData> GetStageData();
    }
}