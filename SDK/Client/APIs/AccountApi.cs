using System.Reflection;
using System.Threading.Tasks;
using Raid.DataModel;

namespace Raid.Client
{
    public class AccountApi : ApiBase<IAccountApi>, IAccountApi
    {
        internal AccountApi(RaidToolkitClient client) : base(client) { }

        public Task<Account> GetAccount(string accountId) =>
            CallMethod<Account>(MethodBase.GetCurrentMethod(), accountId);


        public Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId) =>
            CallMethod<RaidExtractor.Core.AccountDump>(MethodBase.GetCurrentMethod(), accountId);

        public Task<Account[]> GetAccounts() =>
            CallMethod<Account[]>(MethodBase.GetCurrentMethod());


        public Task<Resources> GetAllResources(string accountId) =>
            CallMethod<Resources>(MethodBase.GetCurrentMethod(), accountId);


        public Task<Artifact> GetArtifactById(string accountId, int artifactId) =>
            CallMethod<Artifact>(MethodBase.GetCurrentMethod(), accountId, artifactId);


        public Task<Artifact[]> GetArtifacts(string accountId) =>
            CallMethod<Artifact[]>(MethodBase.GetCurrentMethod(), accountId);


        public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false) =>
            CallMethod<Hero>(MethodBase.GetCurrentMethod(), accountId, heroId, snapshot);


        public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false) =>
            CallMethod<Hero[]>(MethodBase.GetCurrentMethod(), accountId, snapshot);
    }
}