using System;
using System.Linq;
using System.Threading.Tasks;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extension.Realtime
{
    public class RealtimeApi : ApiHandler<IRealtimeApi>, IRealtimeApi
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly IGameInstanceManager InstanceManager;
        private static readonly DataSpec<AccountBase> _Account = new("account");

        public event EventHandler<SerializableEventArgs> AccountListUpdated;
        public event EventHandler<SerializableEventArgs> ViewChanged;
        public event EventHandler<SerializableEventArgs> ReceiveBattleResponse;

        public RealtimeApi(
            ILogger<RealtimeApi> logger,
            IGameInstanceManager instanceManager,
            CachedDataStorage<PersistedDataStorage> storage
            )
            : base(logger)
        {
            Storage = storage;
            InstanceManager = instanceManager;
            InstanceManager.OnAdded += HandleInstanceManagerUpdateEvent;
            InstanceManager.OnRemoved += HandleInstanceManagerUpdateEvent;
            RealtimeService.BattleResultChanged += OnBattleResultChanged;
            RealtimeService.ViewChanged += OnViewChanged;
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            ViewChanged?.Invoke(this, new ViewUpdatedEventArgs(e.Instance.Id, e.ViewMeta.Key.ToString()));
        }

        private void OnBattleResultChanged(object sender, BattleResultsChangedEventArgs e)
        {
            ReceiveBattleResponse?.Invoke(this, new BasicSerializableEventArgs("last-battle-response-updated", e.Instance.Id));
        }

        private void HandleInstanceManagerUpdateEvent(object sender, IGameInstanceManager.GameInstancesUpdatedEventArgs e)
        {
            AccountListUpdated?.Invoke(this, new BasicSerializableEventArgs("account-list-updated"));
        }

        public Task<Account[]> GetConnectedAccounts()
        {
            var accounts = InstanceManager.Instances
                .Select(instance => Account.FromBase(
                    _Account.Get(Storage, new AccountDataContext(instance.Id)),
                    // TODO: Get correct last updated time
                    DateTime.UtcNow)
                );
            return Task.FromResult(accounts.ToArray());
        }

        public Task<ViewInfo> GetCurrentViewInfo(string accountId)
        {
            var viewKey = InstanceManager.GetById(accountId).Properties.GetValue<ViewKey>();
            return Task.FromResult(
                new ViewInfo()
                {
                    ViewId = (int)viewKey,
                    ViewKey = viewKey.ToString()
                }
                );
        }

        public Task<LastBattleDataObject> GetLastBattleResponse(string accountId)
        {
            return Task.FromResult(
                InstanceManager.GetById(accountId).Properties.GetValue<LastBattleDataObject>()
                );
        }
    }
}
