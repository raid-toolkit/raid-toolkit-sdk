using System;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extension.Account
{
    public class AccountInfoProvider : DataProvider<AccountDataContext, AccountBase>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "account";
        public override Version Version => kVersion;
        private readonly CachedDataStorage<PersistedDataStorage> Storage;

        public AccountInfoProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
        {
            ModelScope scope = new(runtime);
            var userWrapper = scope.AppModel._userWrapper;
            var accountData = userWrapper.Account.AccountData;
            var gameSettings = userWrapper.UserGameSettings.GameSettings;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;

            return Storage.Write(context, Key, new AccountBase
            {
                Id = string.Join("_", globalId, socialId).Sha256(),
                Avatar = gameSettings.Avatar.ToString(),
                Name = gameSettings.Name,
                Level = accountData.Level,
                Power = (int)Math.Round(accountData.TotalPower, 0)
            });
        }
    }
}
