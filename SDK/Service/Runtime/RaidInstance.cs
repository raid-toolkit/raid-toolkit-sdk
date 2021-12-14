using System;
using System.Diagnostics;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Service.DataServices;

namespace Raid.Service
{
    public sealed class RaidInstance : IDisposable
    {
        public string Id;
        public Il2CsRuntimeContext Runtime { get; private set; }
        private UserAccount UserAccount;
        private string AccountName;
        private bool HasCheckedStaticData;

        private readonly AppData UserData;
        private readonly ILogger<RaidInstance> Logger;
        private readonly ErrorService ErrorService;
        private readonly IPersistedDataManager<StaticDataContext> StaticDataManager;

        public RaidInstance(
            AppData userData,
            IPersistedDataManager<StaticDataContext> staticDataManager,
            ErrorService errorService,
            ILogger<RaidInstance> logger)
        {
            UserData = userData;
            StaticDataManager = staticDataManager;
            Logger = logger;
            ErrorService = errorService;
        }

        public RaidInstance Attach(Process process)
        {
            Runtime = new Il2CsRuntimeContext(process);
            (Id, AccountName) = GetAccountIdAndName();
            UserAccount = UserData.GetAccount(Id);
            return this;
        }

        public void Update()
        {
            using TrackedOperation updateAccountOp = ErrorService.TrackOperation(ServiceErrorCategory.Account, AccountName, this);

            if (!HasCheckedStaticData)
            {
                var result = StaticDataManager.Update(Runtime, StaticDataContext.Default);
                if (result == UpdateResult.Failed)
                    return;

                HasCheckedStaticData = true;
            }

            if (!UserAccount.Update(Runtime))
                updateAccountOp.Fail(ServiceError.AccountReadError, 10);
        }

        private (string, string) GetAccountIdAndName()
        {
            ModelScope scope = new(Runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return (string.Join('_', globalId, socialId).Sha256(), userWrapper.UserGameSettings.GameSettings.Name);
        }

        public void Dispose()
        {
            Runtime.Dispose();
        }
    }
}
