using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Raid.Toolkit.Extensibility;

public interface IGameInstanceManager
{
	event EventHandler<GameInstanceAddedEventArgs> OnAdded;
	event EventHandler<GameInstanceRemovedEventArgs> OnRemoved;

	void AddInstance(Process process);
	void RemoveInstance(int token);
	bool TryGetById(string id, [NotNullWhen(true)] out ILoadedGameInstance? instance);
	IReadOnlyList<ILoadedGameInstance> Instances { get; }
}
public class GameInstanceAddedEventArgs : EventArgs
{
	public ILoadedGameInstance Instance { get; }
	public GameInstanceAddedEventArgs(ILoadedGameInstance gameInstance)
		=> Instance = gameInstance;
}

public class GameInstanceRemovedEventArgs : EventArgs
{
	public string Id { get; }
	public IGameInstance Instance { get; }
	public GameInstanceRemovedEventArgs(IGameInstance gameInstance, string id)
		=> (Instance, Id) = (gameInstance, id);
}
