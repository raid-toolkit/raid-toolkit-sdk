using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Raid.DataModel;

namespace Raid.Client
{
    public class StaticDataApi : ApiBase<IStaticDataApi>, IStaticDataApi
    {
        internal StaticDataApi(RaidToolkitClient client) : base(client) { }

        public Task<StaticData> GetAllData() =>
            CallMethod<StaticData>(MethodBase.GetCurrentMethod());

        public Task<StaticArenaData> GetArenaData() =>
            CallMethod<StaticArenaData>(MethodBase.GetCurrentMethod());

        public Task<StaticArtifactData> GetArtifactData() =>
            CallMethod<StaticArtifactData>(MethodBase.GetCurrentMethod());

        public Task<StaticHeroData> GetHeroData() =>
            CallMethod<StaticHeroData>(MethodBase.GetCurrentMethod());

        public Task<IReadOnlyDictionary<string, string>> GetLocalizedStrings() =>
            CallMethod<IReadOnlyDictionary<string, string>>(MethodBase.GetCurrentMethod());

        public Task<StaticSkillData> GetSkillData() =>
            CallMethod<StaticSkillData>(MethodBase.GetCurrentMethod());

        public Task<StaticStageData> GetStageData() =>
            CallMethod<StaticStageData>(MethodBase.GetCurrentMethod());
    }
}