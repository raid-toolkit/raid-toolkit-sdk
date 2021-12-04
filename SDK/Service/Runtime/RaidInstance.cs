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
        private int FailuresInARow;

        private readonly UserData UserData;
        private readonly StaticDataCache StaticDataCache;
        private readonly ILogger<RaidInstance> Logger;
        private readonly ErrorService ErrorService;

        public RaidInstance(
            UserData userData,
            StaticDataCache staticDataCache,
            ErrorService errorService,
            ILogger<RaidInstance> logger)
        {
            UserData = userData;
            StaticDataCache = staticDataCache;
            Logger = logger;
            ErrorService = errorService;
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
            ModelScope scope = new(Runtime);
            StaticDataCache.Update(scope);
            if (!StaticDataCache.IsReady)
                return;

            if (UserAccount.Update(scope))
                FailuresInARow = 0;
            else
                ++FailuresInARow;

            if (FailuresInARow > 10)
            {
                string accountName = AccountFacet.ReadValue(UserAccount).Name;
                ErrorService.EmitError(new ErrorEventArgs(ServiceError.AccountReadError, ServiceErrorCategory.Account, accountName, this));
            }
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
