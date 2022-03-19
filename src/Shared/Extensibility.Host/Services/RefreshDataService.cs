using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class DataUpdateSettings
    {
        public int IdleIntervalMs { get; set; }
        public int ActiveIntervalMs { get; set; }
        public int ActiveCooldownMs { get; set; }
    }
    public class RefreshDataService : IHostedService
    {
        private DateTime ActiveUntil = DateTime.UtcNow;

        private readonly ILogger<RefreshDataService> Logger;
        private readonly IOptions<DataUpdateSettings> Settings;

        private readonly ISessionManager SessionManager;
        private readonly IGameInstanceManager GameInstanceManager;
        private readonly PersistedDataManager<StaticDataContext> StaticDataManager;
        private readonly PersistedDataManager<AccountDataContext> AccountDataManager;
        private readonly IHostApplicationLifetime Lifetime;
        private bool HasCheckedStaticData;

        public RefreshDataService(
            ILogger<RefreshDataService> logger,
            IOptions<DataUpdateSettings> settings,
            IGameInstanceManager gameInstanceManager,
            PersistedDataManager<StaticDataContext> staticDataManager,
            PersistedDataManager<AccountDataContext> accountDataManager,
            ISessionManager sessionManager,
            IHostApplicationLifetime lifetime)
        {
            Logger = logger;
            Settings = settings;
            GameInstanceManager = gameInstanceManager;
            AccountDataManager = accountDataManager;
            StaticDataManager = staticDataManager;
            SessionManager = sessionManager;
            Lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Lifetime.ApplicationStarted.Register(() =>
            {
                _ = ExecuteOnceAsync(cancellationToken);
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected async Task ExecuteOnceAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            ActiveUntil = SessionManager.LastSessionActive.AddMilliseconds(Settings.Value.ActiveCooldownMs);
            bool isActive = ActiveUntil > DateTime.UtcNow;
            int nextDelay = isActive ? Settings.Value.ActiveIntervalMs : Settings.Value.IdleIntervalMs;

            var instances = GameInstanceManager.Instances;
            foreach (var instance in instances)
            {
                if (!HasCheckedStaticData)
                {
                    var result = StaticDataManager.Update(instance.Runtime, StaticDataContext.Default);
                    if (result == UpdateResult.Failed)
                        continue;

                    HasCheckedStaticData = true;
                }

                AccountDataManager.Update(instance.Runtime, instance.Id);
            }

            try
            {
                await Task
                    .Delay(nextDelay, token)
                    .ContinueWith((_) => ExecuteOnceAsync(token));
            }
            catch (OperationCanceledException) // expected if the service is shutting down
            { }
        }
    }
}