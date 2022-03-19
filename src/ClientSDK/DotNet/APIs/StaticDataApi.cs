using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class StaticDataApi : ApiBase<IStaticDataApi>, IStaticDataApi
    {
        internal StaticDataApi(RaidToolkitClient client) : base(client) { }

        public Task<StaticData> GetAllData()
        {
            return CallMethod<StaticData>(MethodBase.GetCurrentMethod());
        }

        public Task<StaticArenaData> GetArenaData()
        {
            return CallMethod<StaticArenaData>(MethodBase.GetCurrentMethod());
        }

        public Task<StaticArtifactData> GetArtifactData()
        {
            return CallMethod<StaticArtifactData>(MethodBase.GetCurrentMethod());
        }

        public Task<StaticHeroTypeData> GetHeroData()
        {
            return CallMethod<StaticHeroTypeData>(MethodBase.GetCurrentMethod());
        }

        public Task<IReadOnlyDictionary<string, string>> GetLocalizedStrings()
        {
            return CallMethod<IReadOnlyDictionary<string, string>>(MethodBase.GetCurrentMethod());
        }

        public Task<StaticSkillData> GetSkillData()
        {
            return CallMethod<StaticSkillData>(MethodBase.GetCurrentMethod());
        }

        public Task<StaticStageData> GetStageData()
        {
            return CallMethod<StaticStageData>(MethodBase.GetCurrentMethod());
        }
    }
}
