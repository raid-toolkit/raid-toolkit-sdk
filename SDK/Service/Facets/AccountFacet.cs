using System;
using Raid.DataModel;

namespace Raid.Service
{
    [Facet("account")]
    public class AccountFacet : UserAccountFacetBase<AccountBase, AccountFacet>
    {
        protected override AccountBase Merge(ModelScope scope, AccountBase previous = null)
        {
            var userWrapper = scope.AppModel._userWrapper;
            var accountData = userWrapper.Account.AccountData;
            var gameSettings = userWrapper.UserGameSettings.GameSettings;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;

            return new AccountBase
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
