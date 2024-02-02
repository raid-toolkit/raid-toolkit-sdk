using System;
using System.Diagnostics;

using Client.Model;

using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility.Host;

public class GameInstance : IGameInstance, ILoadedGameInstance, IDisposable
{
	public int Token { get; private set; }
	public string? Id { get; private set; }
	public string? AccountName { get; private set; }
	public string? AvatarUrl { get; private set; }
	public Il2CsRuntimeContext Runtime { get; private set; }

	string ILoadedGameInstance.Id => Id ?? throw new InvalidOperationException();
	string ILoadedGameInstance.AvatarUrl => AvatarUrl ?? throw new InvalidOperationException();

	private bool IsDisposed;

	public GameInstance(Process proc)
	{
		Token = proc.Id;
		Runtime = new(proc);
		Id = AccountName = AvatarUrl = string.Empty;
	}

	public ILoadedGameInstance InitializeOrThrow(Process proc)
	{
		Runtime ??= new(proc);

		var appModel = AppModel._instance.GetValue(Runtime);
		var userWrapper = appModel._userWrapper;
		var socialWrapper = userWrapper.Social.SocialData;
		var globalId = socialWrapper.PlariumGlobalId;
		var socialId = socialWrapper.SocialId;
		AvatarUrl = $"https://raidtoolkit.com/img/avatars/{(int)userWrapper.UserGameSettings.GameSettings.Avatar}.png";
		Id = string.Join('_', globalId, socialId).Sha256();
		AccountName = userWrapper.UserGameSettings.GameSettings.Name;
		return this;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Runtime?.Dispose();
			}
			IsDisposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
