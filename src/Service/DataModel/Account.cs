using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Newtonsoft.Json;
using Raid.Model;
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