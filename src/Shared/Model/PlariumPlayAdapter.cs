using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

#if NET5_0_OR_GREATER
[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif

namespace Raid.Toolkit.Model
{
    public class PlariumPlayAdapter
    {
        public class GameInfo
        {
            public string Name;
            public string Version;
            public string InstallPath;
            public string PlariumPlayPath;
        }

        private class GameStorageEntry
        {
            public int Id { get; set; }
            public int IntegrationType { get; set; }
            [JsonProperty("InsalledGames")]
            public IDictionary<string, string> InstalledGames { get; set; }
        }

        private class GameStorage
        {
            public IDictionary<string, GameStorageEntry> InstalledGames { get; set; }
        }

        private const string kPlariumPlayHive = @"SOFTWARE\PlariumPlayInstaller";
        private const string kInstallFolderKey = @"InstallFolder";
        private const string kGameStoragePath = @"PlariumPlay\gamestorage.gsfn";
        private const string kGameSubpath = @"PlariumPlay\StandaloneApps";
        private const string kPlariumPlayExeSubPath = @"PlariumPlay\PlariumPlay.exe";

        private readonly GameStorage m_gameStorage;
        private readonly string m_installDir;
        public PlariumPlayAdapter()
        {
            m_installDir = (string)Registry.CurrentUser.OpenSubKey(kPlariumPlayHive).GetValue(kInstallFolderKey);
            m_gameStorage = JsonConvert.DeserializeObject<GameStorage>(File.ReadAllText(Path.Combine(m_installDir, kGameStoragePath)));
        }

        public bool TryGetGameVersion(int gameId, string gameName, out GameInfo gameInfo)
        {
            if (!m_gameStorage.InstalledGames.TryGetValue(gameId.ToString(), out GameStorageEntry entry))
            {
                gameInfo = null;
                return false;
            }
            if (!entry.InstalledGames.TryGetValue(gameName, out string version))
            {
                gameInfo = null;
                return false;
            }

            gameInfo = new GameInfo
            {
                InstallPath = Path.Combine(m_installDir, kGameSubpath, gameName),
                Name = gameName,
                Version = version,
                PlariumPlayPath = Path.Combine(m_installDir, kPlariumPlayExeSubPath)
            };
            return true;
        }
    }
}
