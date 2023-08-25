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
    public class RealtimeApi : ApiHandler<IRealtimeApi>, IRealtimeApi, IDisposable
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly IGameInstanceManager InstanceManager;
        private readonly IExtensionHost Host;

        public event EventHandler<SerializableEventArgs> AccountListUpdated;
        public event EventHandler<SerializableEventArgs> ViewChanged;
        public event EventHandler<SerializableEventArgs> ReceiveBattleResponse;

        public RealtimeApi(
            ILogger<RealtimeApi> logger,
            IExtensionHost host,
            IGameInstanceManager instanceManager,
            CachedDataStorage<PersistedDataStorage> storage
            )
            : base(logger)
        {
            Host = host;
            Storage = storage;
            InstanceManager = instanceManager;
            InstanceManager.OnAdded += HandleInstanceManagerUpdateEvent;
            InstanceManager.OnRemoved += HandleInstanceManagerUpdateEvent;
            RealtimeService.BattleResultChanged += OnBattleResultChanged;
            RealtimeService.ViewChanged += OnViewChanged;
            RealtimeService.Enabled = true; // enable on first use
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            ViewChanged?.Raise(this, new ViewUpdatedEventArgs(e.Instance.Id, e.ViewMeta.Key.ToString()));
        }

        private void OnBattleResultChanged(object sender, BattleResultsChangedEventArgs e)
        {
            ReceiveBattleResponse?.Raise(this, new BasicSerializableEventArgs("last-battle-response-updated", e.Instance.Id));
        }

        private void HandleInstanceManagerUpdateEvent(object sender, IGameInstanceManager.GameInstancesUpdatedEventArgs e)
        {
            AccountListUpdated?.Raise(this, new BasicSerializableEventArgs("account-list-updated"));
        }

        public Task<Account[]> GetConnectedAccounts()
        {
            return Task.FromResult(
                Host.GetAccounts()
                    .Where(account => account.IsOnline)
                    .Select(account => Account.FromBase(account.AccountInfo, DateTime.UtcNow))
                    .ToArray());
        }

        public Task<ViewInfo> GetCurrentViewInfo(string accountId)
        {
            var viewKey = InstanceManager.GetById(accountId).Properties.GetValue<ViewKey>();
            return Task.FromResult(
                new ViewInfo()
                {
                    ViewId = (int)viewKey,
                    ViewKey = viewKey.ToString()
                });
        }

        public Task<LastBattleDataObject> GetLastBattleResponse(string accountId)
        {
            return Task.FromResult(
                InstanceManager.GetById(accountId).Properties.GetValue<LastBattleDataObject>()
                );
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
