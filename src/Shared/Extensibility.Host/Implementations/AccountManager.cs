using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Client.Model;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host
{
    public class AccountManager : IAccountManager
    {
        private readonly object _syncRoot = new();
        private readonly Dictionary<string, AccountInstance> AccountMap = new();
        private readonly List<IAccountExtensionFactory> Factories = new();

        private readonly IServiceProvider ServiceProvider;
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly ILogger<AccountManager> Logger;
        private readonly IGameInstanceManager GameInstanceManager;

        public IEnumerable<IAccount> Accounts => AccountMap.Values;

        public AccountManager(
            IServiceProvider serviceProvider,
            ILogger<AccountManager> logger,
            IGameInstanceManager gameInstanceManager,
            CachedDataStorage<PersistedDataStorage> storage)
        {
            ServiceProvider = serviceProvider;
            Logger = logger;
            GameInstanceManager = gameInstanceManager;
            Storage = storage;

            GameInstanceManager.OnAdded += GameInstanceManager_OnAdded;
            GameInstanceManager.OnRemoved += GameInstanceManager_OnRemoved;
            InitializeFromStorage();
        }

        private bool TryGetAccount(string accountId, [NotNullWhen(true)] out AccountInstance? account)
        {
            lock (_syncRoot)
                return AccountMap.TryGetValue(accountId, out account);
        }

        private void GameInstanceManager_OnAdded(object? sender, IGameInstanceManager.GameInstancesUpdatedEventArgs e)
        {
            if (TryGetAccount(e.Instance.Id, out AccountInstance? account))
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
            if (TryGetAccount(e.Instance.Id, out AccountInstance? account))
                account.OnDisconnected();
        }

        private void InitializeFromStorage()
        {
            foreach (string accountId in CachedDataStorage.GetKeys(new AccountDirectoryContext()))
            {
                LoadAccount(accountId);
            }
        }

        private void LoadAccount(string accountId)
        {
            AccountInstance account = ActivatorUtilities.CreateInstance<AccountInstance>(ServiceProvider, accountId);
            lock (_syncRoot)
            {
                foreach (IAccountExtensionFactory factory in Factories)
                {
                    AccountMap[accountId].AddExtension(factory);
                }
                AccountMap.TryAdd(accountId, account);
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
