using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

using Client.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host
{
    public class AccountManager : PollingBackgroundService, IAccountManager
    {
        private readonly static TimeSpan kPollInterval = new(0, 0, 0, 0, 50);
        private protected override TimeSpan PollInterval => kPollInterval;

        private readonly object _syncRoot = new();
        private readonly Dictionary<string, AccountInstance> AccountMap = new();
        private readonly List<IAccountExtensionFactory> Factories = new();

        private readonly IServiceProvider ServiceProvider;
        private readonly PersistedDataStorage Storage;
        private readonly IGameInstanceManager GameInstanceManager;

        public IEnumerable<IAccount> Accounts => AccountMap.Values;

        public AccountManager(
            IServiceProvider serviceProvider,
            ILogger<AccountManager> logger,
            IGameInstanceManager gameInstanceManager,
            PersistedDataStorage storage)
            : base(logger)
        {
            ServiceProvider = serviceProvider;
            GameInstanceManager = gameInstanceManager;
            Storage = storage;

            GameInstanceManager.OnAdded += GameInstanceManager_OnAdded;
            GameInstanceManager.OnRemoved += GameInstanceManager_OnRemoved;
            InitializeFromStorage();
        }

        public bool TryGetAccount(string accountId, [NotNullWhen(true)] out AccountInstance? account)
        {
            lock (_syncRoot)
                return AccountMap.TryGetValue(accountId, out account);
        }

        public bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account)
        {
            AccountInstance? result;
            bool found = false;
            lock (_syncRoot)
            {
                found = AccountMap.TryGetValue(accountId, out result);
            }
            account = result;
            return found;
        }

        private void GameInstanceManager_OnAdded(object? sender, IGameInstanceManager.GameInstancesUpdatedEventArgs e)
        {
            if (TryGetAccount(e.Instance.Id, out IAccount? value) && value is AccountInstance account)
            {
                account.OnConnected(e.Instance);
            }
            else
            {
                var appModel = Client.App.SingleInstance<AppModel>._instance.GetValue(e.Instance.Runtime);
                var userWrapper = appModel._userWrapper;
                var accountData = userWrapper.Account.AccountData;
                var gameSettings = userWrapper.UserGameSettings.GameSettings;
                var socialWrapper = userWrapper.Social.SocialData;
                var globalId = socialWrapper.PlariumGlobalId;
                var socialId = socialWrapper.SocialId;
                Storage.Write(new AccountDataContext(e.Instance.Id), "info.json", new AccountBase
                {
                    Id = string.Join("_", globalId, socialId).Sha256(),
                    Avatar = gameSettings.Avatar.ToString(),
                    AvatarId = ((int)gameSettings.Avatar).ToString(),
                    Name = gameSettings.Name,
                    Level = accountData.Level,
                    Power = (int)Math.Round(accountData.TotalPower, 0)
                });
                LoadAccount(e.Instance.Id);
            }
        }

        private void GameInstanceManager_OnRemoved(object? sender, IGameInstanceManager.GameInstancesUpdatedEventArgs e)
        {
            if (TryGetAccount(e.Instance.Id, out IAccount? value) && value is AccountInstance account)
                account.OnDisconnected();
        }

        private void InitializeFromStorage()
        {
            foreach (string accountId in Storage.GetKeys(new AccountDirectoryContext()))
            {
                LoadAccount(accountId);
            }
        }

        private void LoadAccount(string accountId)
        {
            AccountInstance account;
            try
            {
                account = ActivatorUtilities.CreateInstance<AccountInstance>(ServiceProvider, accountId);
            }
            catch
            {
                // account not fully populated?
                return;
            }

            lock (_syncRoot)
            {
                foreach (IAccountExtensionFactory factory in Factories)
                {
                    account.AddExtension(factory);
                }
                AccountMap.TryAdd(accountId, account);
            }
        }

        protected override async Task ExecuteOnceAsync(CancellationToken token)
        {
            AccountInstance[] accounts;
            lock (_syncRoot)
            {
                accounts = AccountMap.Values.ToArray();
            }
            foreach (AccountInstance account in accounts)
            {
                if (!AccountMap.ContainsValue(account))
                    continue;

                Stopwatch swScoped = Stopwatch.StartNew();
                Logger.LogInformation("Background processing for account {account}", account.Id);
                try
                {
                    await account.Tick();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed background processing for account {account}", account.Id);
                }
                Logger.LogInformation("Background processing for account {account} completed in {ms}ms", account.Id, swScoped.ElapsedMilliseconds);
            }
        }

        public void RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory
        {
            lock (_syncRoot)
            {
                Factories.Add(factory);
                foreach (AccountInstance account in AccountMap.Values)
                {
                    account.AddExtension(factory);
                }
            }
        }

        public void UnregisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory
        {
            lock (_syncRoot)
            {
                Factories.Remove(factory);

                foreach (AccountInstance account in AccountMap.Values)
                {
                    account.RemoveExtension(factory);
                }
            }
        }

    }
}
