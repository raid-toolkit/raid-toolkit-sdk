using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Account;

namespace Raid.Service.DataModel
{
    public class Account
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("avatar")]
        public UserAvatarId Avatar;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("level")]
        public int Level;

        [JsonProperty("power")]
        public int Power;
    }
}