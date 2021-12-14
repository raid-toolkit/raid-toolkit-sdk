using System;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("account")]
    public class AccountDataObject : AccountBase
    {
    }

    public class AccountInfoProvider : DataProviderBase<AccountDataContext, AccountDataObject>
    {
        public AccountInfoProvider(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, AccountDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, AccountDataContext context)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var accountData = userWrapper.Account.AccountData;
            var gameSettings = userWrapper.UserGameSettings.GameSettings;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;

            return PrimaryProvider.Write(context, new AccountDataObject
            {
                Id = string.Join('_', globalId, socialId).Sha256(),
                Avatar = gameSettings.Avatar.ToString(),
                Name = gameSettings.Name,
                Level = accountData.Level,
                Power = (int)Math.Round(accountData.TotalPower, 0)
            });
        }
    }
}
