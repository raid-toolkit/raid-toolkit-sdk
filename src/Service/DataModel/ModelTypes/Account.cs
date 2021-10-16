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

        internal static IReadOnlyDictionary<int, ArtifactSetKind> ToDictionary(Func<object, int> p1, Func<object, object> p2)
        {
            throw new NotImplementedException();
        }
    }
}