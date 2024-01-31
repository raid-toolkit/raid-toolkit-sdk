using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility;

[Obsolete("Not intended for public use")]
public interface IGameInstanceManager
{
    event EventHandler<GameInstancesUpdatedEventArgs> OnAdded;
    event EventHandler<GameInstancesUpdatedEventArgs> OnRemoved;

    void AddInstance(Process process);
    void RemoveInstance(int token);
    [Obsolete("Use TryGetById instead.")] IGameInstance GetById(string id);
    bool TryGetById(string id, [NotNullWhen(true)] out IGameInstance? instance);
    IReadOnlyList<IGameInstance> Instances { get; }

    class GameInstancesUpdatedEventArgs : EventArgs
    {
        public IGameInstance Instance { get; }
        public GameInstancesUpdatedEventArgs(IGameInstance gameInstance)
            => Instance = gameInstance;
    }
}
