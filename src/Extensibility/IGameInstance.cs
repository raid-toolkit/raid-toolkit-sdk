using System;
using System.Diagnostics;

using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility;

public interface IGameInstance : IDisposable
{
	int Token { get; }
	Il2CsRuntimeContext Runtime { get; }

	ILoadedGameInstance InitializeOrThrow(Process proc);
}

public interface ILoadedGameInstance : IGameInstance
{
	string Id { get; }
	string AvatarUrl { get; }
}
