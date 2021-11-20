using System;
using System.Diagnostics;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;

namespace Raid.Service
{
    public sealed class RaidInstance : IDisposable
    {
        public string Id;
        private Il2CsRuntimeContext Runtime;
        private UserAccount UserAccount;

        private readonly UserData UserData;
        private readonly StaticDataCache StaticDataCache;
        private readonly ILogger<RaidInstance> Logger;

        public RaidInstance(
                UserData userData,
            StaticDataCache staticDataCache,
            ILogger<RaidInstance> logger)
        {
            UserData = userData;
            StaticDataCache = staticDataCache;
            Logger = logger;
        }

        public RaidInstance Attach(Process process)
        {
            Runtime = new Il2CsRuntimeContext(process);
            Id = GetAccountId();
            UserAccount = UserData.GetAccount(Id);
            return this;
        }

        public void Update()
        {
            StaticDataCache.Update(Runtime);
            if (!StaticDataCache.IsReady)
            {
                return;
            }
            UserAccount.Update(new(Runtime));
        }

        private string GetAccountId()
        {
            ModelScope scope = new(Runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return string.Join('_', globalId, socialId).Sha256();
        }

        public void Dispose()
        {
            Runtime.Dispose();
        }
    }
}
