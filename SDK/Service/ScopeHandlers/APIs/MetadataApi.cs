using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using Raid.Model;

namespace Raid.Service
{
    [PublicApi("api:metadata")]
    internal class MetadataApi : ApiHandler<MetadataApi>
    {
        private static readonly string s_installPath;
        static MetadataApi()
        {
            PlariumPlayAdapter pp = new();
            if (!pp.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo))
            {
                throw new InvalidOperationException("Game is not installed");
            }
            s_installPath = Path.Join(gameInfo.InstallPath, gameInfo.Version);
        }

        public MetadataApi(ILogger<MetadataApi> logger) : base(logger) { }

        [PublicApi("getInstallPath")]
        public static string getInstallPath()
        {
            return s_installPath;
        }
    }
}
