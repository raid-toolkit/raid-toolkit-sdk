using Client.RaidApp;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class BattleResultsChangedEventArgs : EventArgs
    {
        public IGameInstance Instance { get; }
        public BattleResultsChangedEventArgs(IGameInstance instance)
        {
            Instance = instance;
        }
    }
    internal class RealtimeService : IBackgroundService
    {
        private static readonly TimeSpan kPollInterval = new(0, 0, 0, 0, 100);
        private readonly ILogger<RealtimeService> Logger;
        public TimeSpan PollInterval => kPollInterval;

        public static event EventHandler<ViewChangedEventArgs> ViewChanged;
        public static event EventHandler<BattleResultsChangedEventArgs> BattleResultChanged;

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
            if (instance.Properties.GetValue<ViewInfo>()?.ViewId != (int)topView.Key)
            {
                instance.Properties.SetValue<ViewInfo>(new() { ViewId = (int)topView.Key, ViewKey = topView.Key.ToString() });
                ViewChanged?.Invoke(this, new(instance, topView));
            }
            return Task.CompletedTask;
        }

        private void UpdateLastView(IGameInstance instance, ModelScope scope)
        {
            if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
                return;

            ViewMeta topView = viewMaster._views[^1];
            if (instance.Properties.GetValue<ViewKey>() == topView.Key)
                return;

            instance.Properties.SetValue<ViewKey>(topView.Key);
            ViewChanged?.Invoke(this, new(instance, topView));
        }

        private void UpdateLastBattleState(IGameInstance instance, ModelScope scope)
        {
            var response = scope.AppModel._userWrapper.Battle.BattleData.LastResponse;
            if (instance.Properties.GetValue<DateTime>("lastBattleResponse") == response.StartTime.Value)
                return;

            instance.Properties.SetValue<DateTime>("lastBattleResponse", response.StartTime.Value);
            instance.Properties.SetValue<LastBattleDataObject>(new()
            {
                BattleKindId = response.BattleKindId.ToString(),
                HeroesExperience = response.HeroesExperience,
                HeroesExperienceAdded = response.HeroesExperienceAdded,
                Turns = response.Turns.ToNullable(),
                TournamentPointsByStateId = response.TournamentPointsByStateId.UnderlyingDictionary,
                GivenDamage = new()
                {
                    DemonLord = response.GivenDamageToAllianceBoss.ToNullable(),
                    Hydra = response.GivenDamageToAllianceHydra.ToNullable(),
                },
                MasteryPointsByHeroId = response.MasteryPointsByHeroId?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (Dictionary<string, int>)kvp.Value.UnderlyingDictionary.ToModel())
            });
            BattleResultChanged?.Invoke(this, new(instance));
        }
    }
}