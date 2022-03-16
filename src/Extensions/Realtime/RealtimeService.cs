using Client.RaidApp;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility;
using System;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Realtime
{
    internal class ViewChangedEventArgs : EventArgs
    {
        public IGameInstance Instance { get; }
        public ViewMeta ViewMeta { get; }
        public ViewChangedEventArgs(IGameInstance instance, ViewMeta viewMeta)
        {
            Instance = instance;
            ViewMeta = viewMeta;
        }
    }
    internal class RealtimeService : IBackgroundService
    {
        private static readonly TimeSpan kPollInterval = new(0, 0, 0, 0, 100);
        private readonly ILogger<RealtimeService> Logger;
        public TimeSpan PollInterval => kPollInterval;

        public static event EventHandler<ViewChangedEventArgs> ViewChanged;

        public RealtimeService(ILogger<RealtimeService> logger)
        {
            Logger = logger;
        }

        public Task Tick(IGameInstance instance)
        {
            var process = instance.Runtime.TargetProcess;

            ModelScope scope = new(instance.Runtime);
            if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
                return Task.CompletedTask;

            ViewMeta topView = viewMaster._views[^1];
            if (instance.Properties.GetValue<ViewKey>() != topView.Key)
            {
                instance.Properties.SetValue<ViewKey>(topView.Key);
                ViewChanged?.Invoke(this, new(instance, topView));
            }
            return Task.CompletedTask;
        }
    }
}