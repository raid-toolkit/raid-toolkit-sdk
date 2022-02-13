using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Raid.DataModel;
using Raid.Service.DataServices;

namespace Raid.Service
{
    internal class RuntimeApi : ApiHandler<IRuntimeApi>, IRuntimeApi
    {
        private readonly RaidInstanceFactory InstanceFactory;
        private readonly AccountDataBundle AccountData;
        private readonly LastBattleProvider LastBattleProvider;
        public RuntimeApi(
            ILogger<RuntimeApi> logger,
            RaidInstanceFactory instanceFactory,
            EventService eventService,
            LastBattleProvider lastBattleProvider,
            AccountDataBundle accountData)
            : base(logger)
        {
            InstanceFactory = instanceFactory;
            AccountData = accountData;
            LastBattleProvider = lastBattleProvider;
            // TODO: add+subscribe to add/delete events from instances instead of accounts
            eventService.OnAccountUpdated += OnAccountUpdated;
        }

        private void OnAccountUpdated(object sender, AccountUpdatedEventArgs e)
        {
            Updated?.Invoke(this, e);
        }

        [PublicApi("updated")]
#pragma warning disable 0067
        public event EventHandler<SerializableEventArgs> Updated;
#pragma warning restore 0067

        public Task<Account[]> GetConnectedAccounts()
        {
            return Task.FromResult(InstanceFactory.Instances.Values.Select(AccountFromInstance).ToArray());
        }

        public Task<JObject> GetLastBattleResponse(string accountId)
        {
            return Task.FromResult(JObject.FromObject((object)LastBattleProvider.GetValue(accountId) ?? JValue.CreateNull()));
        }

        private Account AccountFromInstance(RaidInstance instance)
        {
            AccountBase result = AccountData.AccountInfo.GetValue(new(instance.Id));
            return Account.FromBase(result, instance.UserAccount.LastUpdated);
        }
    }
}
