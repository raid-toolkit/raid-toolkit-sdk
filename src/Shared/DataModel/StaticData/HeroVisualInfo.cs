using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.DataModel;

public class HeroVisualInfo
{
    [JsonProperty("avatar")]
    public string? AvatarName;

    [JsonProperty("model")]
    public string? ModelName;

    [JsonProperty("showcase")]
    public string? ShowcaseSceneName;
}
