using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raid.Toolkit.Extensibility
{
    public interface IGameInstanceManager
    {
        public class GameInstancesUpdatedEventArgs : EventArgs
        {
            public IGameInstance Instance { get; }
            public GameInstancesUpdatedEventArgs(IGameInstance gameInstance)
                => Instance = gameInstance;
        }

        event EventHandler<GameInstancesUpdatedEventArgs> OnAdded;
        event EventHandler<GameInstancesUpdatedEventArgs> OnRemoved;

        void AddInstance(Process process);
        void RemoveInstance(int token);
        IGameInstance GetById(string id);
        IReadOnlyList<IGameInstance> Instances { get; }
    }
}
