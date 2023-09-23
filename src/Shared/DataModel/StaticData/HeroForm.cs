using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Raid.Toolkit.DataModel.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel;

public class HeroForm
{
    [JsonProperty("role"), JsonConverter(typeof(StringEnumConverter))]
    public HeroRole? Role;

    [JsonProperty("unscaledStats")]
    public Stats? UnscaledStats;

    [JsonProperty("skillTypeIds")]
    public int[]? SkillTypeIds;

    [JsonProperty("visualInfosBySkin")]
    public Dictionary<int, HeroVisualInfo>? VisualInfosBySkin;
}
