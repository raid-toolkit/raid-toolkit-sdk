using System;
using System.Diagnostics;
using Il2CppToolkit.Runtime;
using Raid.Service.DataServices;

namespace Raid.Service
{
    public sealed class RaidInstance : IDisposable
    {
        public string Id;
        public Il2CsRuntimeContext Runtime { get; private set; }
        public UserAccount UserAccount;
        private string AccountName;
        private bool HasCheckedStaticData;

        private readonly AppData UserData;
        private readonly ErrorService ErrorService;
        private readonly IPersistedDataManager<StaticDataContext> StaticDataManager;
        private readonly IPersistedDataManager<RuntimeDataContext> RuntimeDataManager;

        public RaidInstance(
            AppData userData,
            IPersistedDataManager<RuntimeDataContext> runtimeDataManager,
            IPersistedDataManager<StaticDataContext> staticDataManager,
            ErrorService errorService)
        {
            UserData = userData;
            StaticDataManager = staticDataManager;
            RuntimeDataManager = runtimeDataManager;
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

            RuntimeDataManager.Update(Runtime, Id);
            if (!UserAccount.Update(Runtime))
                updateAccountOp.Fail(ServiceError.AccountReadError, 25);
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
