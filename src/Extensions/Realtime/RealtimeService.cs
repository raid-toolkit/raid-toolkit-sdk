using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.RaidApp;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;

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
        public TimeSpan PollInterval => kPollInterval;
        public static bool Enabled { get; set; }

        public static event EventHandler<ViewChangedEventArgs> ViewChanged;
        public static event EventHandler<BattleResultsChangedEventArgs> BattleResultChanged;

        public RealtimeService(ILogger<RealtimeService> logger)
        {
        }

        public Task Tick(IGameInstance instance)
        {
            if (!Enabled)
                return Task.CompletedTask;

            ModelScope scope = new(instance.Runtime);

            UpdateLastView(instance, scope);
            UpdateLastBattleState(instance, scope);
            return Task.CompletedTask;
        }

        private void UpdateLastView(IGameInstance instance, ModelScope scope)
        {
            if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
                return;

            if (viewMaster._views.Count == 0)
                return;

            ViewMeta topView = viewMaster._views[^1];
            if (instance.Properties.GetValue<ViewKey>() != topView.Key)
            {
                instance.Properties.SetValue<ViewKey>(topView.Key);
                ViewChanged?.Raise(this, new(instance, topView));
                return;
            }
        }

        private void UpdateLastBattleState(IGameInstance instance, ModelScope scope)
        {
            var response = scope.AppModel._userWrapper.Battle.BattleData.LastResponse;
            if (instance.Properties.GetValue<DateTime>("lastBattleResponse") == response.StartTime)
                return;

            instance.Properties.SetValue<DateTime>("lastBattleResponse", response.StartTime);
            instance.Properties.SetValue<LastBattleDataObject>(new()
            {
                BattleKindId = response.BattleKindId.ToString(),
                HeroesExperience = response.HeroesExperience,
                HeroesExperienceAdded = response.HeroesExperienceAdded,
                Turns = response.Turns,
                TournamentPointsByStateId = response.TournamentPointsByStateId.UnderlyingDictionary,
                GivenDamage = new()
                {
                    DemonLord = response.GivenDamageToAllianceBoss,
                    Hydra = response.GivenDamageToAllianceHydra,
                },
                MasteryPointsByHeroId = response.MasteryPointsByHeroId?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (Dictionary<string, int>)kvp.Value.UnderlyingDictionary.ToModel())
            });
            BattleResultChanged?.Raise(this, new(instance));
        }
    }
}
