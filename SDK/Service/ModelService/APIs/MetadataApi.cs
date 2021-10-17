using System;
using System.IO;
using Raid.Model;

namespace Raid.Service
{
    [PublicApi("api:metadata")]
    internal class MetadataApi : ApiHandler
    {
        private static string s_installPath;
        static MetadataApi()
        {
            PlariumPlayAdapter pp = new();
            if (!pp.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo))
            {
                throw new InvalidOperationException("Game is not installed");
            }
            s_installPath = Path.Join(gameInfo.InstallPath, gameInfo.Version);
        }

        [PublicApi("getInstallPath")]
        public string getInstallPath() => s_installPath;
    }
}