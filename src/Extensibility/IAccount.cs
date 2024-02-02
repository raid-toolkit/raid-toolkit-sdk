using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Il2CppToolkit.Runtime;

using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility;

public class AccountEventArgs : EventArgs
{
	public AccountEventArgs() { }
}

public interface IAccount
{
	event EventHandler<AccountEventArgs> OnConnected;
	event EventHandler<AccountEventArgs> OnDisconnected;

	string Id { get; }
	string AccountName { get; }
	string AvatarUrl { get; }

	AccountBase AccountInfo { get; }

	[MemberNotNullWhen(true, nameof(Runtime))] bool IsOnline { get; }
	Il2CsRuntimeContext? Runtime { get; }

	bool TryGetApi<T>([NotNullWhen(true)] out T? api) where T : class;
}

public class AccountBase
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use parameterized constructor")]
	public AccountBase()
	{
		Id = Avatar = AvatarId = Name = string.Empty;
	}

	public AccountBase(string id, string avatar, string avatarId, string name, int level, int power)
	{
		Id = id;
		Avatar = avatar;
		AvatarId = avatarId;
		Name = name;
		Level = level;
		Power = power;
	}

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("avatar")]
	public string Avatar { get; set; }

	[JsonProperty("avatarId")]
	public string AvatarId { get; set; }

	[JsonProperty("avatarUrl")]
	public string AvatarUrl => $"https://raidtoolkit.com/img/avatars/{AvatarId}.png";

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("level")]
	public int Level { get; set; }

	[JsonProperty("power")]
	public int Power { get; set; }
}
