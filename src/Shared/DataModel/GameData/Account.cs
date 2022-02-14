using System;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class AccountBase
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("avatar")]
        public string Avatar;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("power")]
        public int Power;
    }
    public class Account : AccountBase
    {
        [JsonProperty("lastUpdated")]
        public DateTime? LastUpdated;

        public static Account FromBase(AccountBase accountBase, DateTime? lastUpdated)
        {
            return new Account()
            {
                Id = accountBase.Id,
                Name = accountBase.Name,
                Level = accountBase.Level,
                Avatar = accountBase.Avatar,
                Power = accountBase.Power,
                LastUpdated = lastUpdated
            };
        }
    }
}
