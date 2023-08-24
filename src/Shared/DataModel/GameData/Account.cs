using System;
using Newtonsoft.Json;

namespace Raid.Toolkit.DataModel
{
    public class AccountBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatarId")]
        public string AvatarId { get; set; }

        [JsonProperty("avatarUrl")]
        public string AvatarUrl => $"https://raidtoolkit.com/img/avatars/{AvatarId}.png";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("power")]
        public int Power { get; set; }
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
                AvatarId = accountBase.AvatarId,
                Power = accountBase.Power,
                LastUpdated = lastUpdated
            };
        }
    }
}
