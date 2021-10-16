using System;
using System.Linq;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [PublicApi("account-api")]
    internal class AccountApi : ApiHandler
    {
        [PublicApi("updated")]
        public event EventHandler<SerializableEventArgs> Updated;

        [PublicApi("dump")]
        public RaidExtractor.Core.AccountDump GetAccountDump(string accountId)
        {
            return RaidExtractor.Core.Extractor.DumpAccount(UserData.Instance.GetAccount(accountId));
        }

        [PublicApi("getAccounts")]
        public Account[] GetAccounts()
        {
            return UserData.Instance.UserAccounts.Select(AccountFacet.ReadValue).ToArray();
        }

        [PublicApi("accountInfo")]
        public Account GetAccount(string accountId)
        {
            return AccountFacet.ReadValue(UserData.Instance.GetAccount(accountId));
        }

        [PublicApi("getArtifacts")]
        public Artifact[] GetArtifacts(string accountId)
        {
            return ArtifactsFacet.ReadValue(UserData.Instance.GetAccount(accountId)).Values.ToArray();
        }

        [PublicApi("getArtifactById")]
        public Artifact GetArtifactById(string accountId, int artifactId)
        {
            return ArtifactsFacet.ReadValue(UserData.Instance.GetAccount(accountId))[artifactId];
        }

        [PublicApi("getHeroes")]
        public Hero[] GetHeroes(string accountId, bool snapshot = false)
        {
            // TODO: Snapshots
            return HeroesFacet.ReadValue(UserData.Instance.GetAccount(accountId)).Values.ToArray();
        }

        [PublicApi("getHeroById")]
        public Hero GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            return HeroesFacet.ReadValue(UserData.Instance.GetAccount(accountId))[heroId];
        }

        [PublicApi("getAllResources")]
        public Resources GetAllResources(string accountId)
        {
            return ResourcesFacet.ReadValue(UserData.Instance.GetAccount(accountId));
        }
    }
}