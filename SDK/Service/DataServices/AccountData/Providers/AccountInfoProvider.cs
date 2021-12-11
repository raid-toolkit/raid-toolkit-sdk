using System;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("account")]
    public class AccountDataObject : AccountBase
    {
    }

    [DataProvider]
    public class AccountInfoProvider : AccountDataProviderBase<AccountDataObject>
    {
        public AccountInfoProvider(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, AccountDataObject> storage)
            : base(storage)
        {
        }

        public override AccountDataObject Update(ModelScope scope, AccountDataContext context)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var accountData = userWrapper.Account.AccountData;
            var gameSettings = userWrapper.UserGameSettings.GameSettings;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;

            return new AccountDataObject
            {
                Id = string.Join('_', globalId, socialId).Sha256(),
                Avatar = gameSettings.Avatar.ToString(),
                Name = gameSettings.Name,
                Level = accountData.Level,
                Power = (int)Math.Round(accountData.TotalPower, 0)
            };
        }
    }
}
