using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.Win32;

using Newtonsoft.Json;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Raid.Toolkit.Loader;

public class PlariumPlayAdapter
{
	public class GameInfo
	{
		public string? Name;
		public string? Version;
		public string? InstallPath;
		public string? PlariumPlayPath;
	}

	private class GameStorageEntry
	{
		public int Id { get; set; }
		public int IntegrationType { get; set; }
		public string? InstallationPath { get; set; }
		[JsonProperty("InsalledGames")]
		public IDictionary<string, string> InstalledGames { get; set; } = new Dictionary<string, string>();
	}

	private class GameStorage
	{
		public IDictionary<string, GameStorageEntry> InstalledGames { get; set; } = new Dictionary<string, GameStorageEntry>();
	}

	private const string kPlariumPlayHive = @".DEFAULT\SOFTWARE\PlariumPlayInstaller";
	private const string kInstallFolderKey = @"InstallFolder";
	private const string kGameStoragePath = @"gamestorage.gsfn";
	private const string kGameSubpath = @"StandaloneApps";
	private const string kPlariumPlayExeSubPath = @"PlariumPlay.exe";

	private readonly GameStorage? m_gameStorage;

	private readonly string? m_installDir;
	public PlariumPlayAdapter()
	{
		m_installDir = (string?)Registry.Users.OpenSubKey(kPlariumPlayHive)?.GetValue(kInstallFolderKey);
		if (!string.IsNullOrEmpty(m_installDir))
		{
			m_gameStorage = JsonConvert.DeserializeObject<GameStorage>(File.ReadAllText(Path.Combine(m_installDir, kGameStoragePath)));
		}
	}

	public bool TryGetGameVersion(int gameId, string gameName, [NotNullWhen(true)] out GameInfo? gameInfo)
	{
		if (m_gameStorage?.InstalledGames.TryGetValue(gameId.ToString(), out GameStorageEntry? entry) != true)
		{
			gameInfo = null;
			return false;
		}
		if (entry?.InstalledGames.TryGetValue(gameName, out string? version) != true)
		{
			gameInfo = null;
			return false;
		}

		if (string.IsNullOrEmpty(m_installDir))
		{
			gameInfo = null;
			return false;
		}

		string defaultInstallDir = Path.Combine(m_installDir, kGameSubpath);
		gameInfo = new GameInfo
		{
			InstallPath = Path.Combine(entry.InstallationPath ?? defaultInstallDir, gameName),
			Name = gameName,
			Version = version,
			PlariumPlayPath = Path.Combine(m_installDir, kPlariumPlayExeSubPath)
		};
		return true;
	}
}
