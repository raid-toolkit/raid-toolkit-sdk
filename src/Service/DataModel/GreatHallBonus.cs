using System.Collections.Generic;
using Newtonsoft.Json;
using SharedModel.Meta.Heroes;

namespace Raid.Service.DataModel
{
    public class GreatHallBonus
    {
        [JsonProperty("affinity")]
        public Element Affinity;

        [JsonProperty("bonus")]
        public List<StatBonus> Bonus;
    }
}