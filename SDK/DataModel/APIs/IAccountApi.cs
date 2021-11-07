using System;
using System.Threading.Tasks;

namespace Raid.DataModel
{
    public class AccountChangedEventArgs : EventArgs
    {
    }
    [PublicApi("account-api")]
    public interface IAccountApi
    {
        [PublicApi("change")]
        event EventHandler<AccountChangedEventArgs> OnChange;

        [PublicApi("getAccountDump")]
        Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId);

        [PublicApi("getAllResources")]
        Task<Resources> GetAllResources(string accountId);

        [PublicApi("getAccounts")]
        Task<Account[]> GetAccounts();

        [PublicApi("accountInfo")]
        Task<Account> GetAccount(string accountId);

        [PublicApi("getArtifacts")]
        Task<Artifact[]> GetArtifacts(string accountId);

        [PublicApi("getArtifactById")]
        Task<Artifact> GetArtifactById(string accountId, int artifactId);

        [PublicApi("getHeroes")]
        Task<Hero[]> GetHeroes(string accountId, bool snapshot = false);

        [PublicApi("getHeroById")]
        Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false);
    }
}