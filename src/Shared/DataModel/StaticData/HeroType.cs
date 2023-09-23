using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raid.Toolkit.DataModel.Enums;

using System;
using System.Linq;

namespace Raid.Toolkit.DataModel;

public class HeroType
{
    [JsonProperty("typeId")]
    public int TypeId = - 1;

    [JsonProperty("name")]
    public LocalizedText? Name;

    [JsonProperty("shortName")]
    public LocalizedText? ShortName;

    [JsonProperty("affinity"), JsonConverter(typeof(StringEnumConverter))]
    public Element? Affinity;

    [JsonProperty("faction"), JsonConverter(typeof(StringEnumConverter))]
    public HeroFraction? Faction;

    [Obsolete("Use forms instead"), JsonProperty("role"), JsonConverter(typeof(StringEnumConverter))]
    public HeroRole? Role;

    [JsonProperty("rarity"), JsonConverter(typeof(StringEnumConverter))]
    public HeroRarity? Rarity;

    [JsonProperty("ascended")]
    public int Ascended = 0;

    [Obsolete("Use forms instead"), JsonProperty("modelName")]
    public string? ModelName;

    [Obsolete("Use forms instead"), JsonProperty("avatarKey")]
    public string? AvatarKey;

    [JsonProperty("leaderSkill")]
    public LeaderStatBonus? LeaderSkill;

    [Obsolete("Use forms instead"), JsonProperty("skillTypeIds")]
    public int[]? SkillTypeIds;

    [Obsolete("Use forms instead"), JsonProperty("unscaledStats")]
    public Stats? UnscaledStats
    {
        get => _UnscaledStats ?? Forms?.First().UnscaledStats;
        set => _UnscaledStats = value;
    }
    private Stats? _UnscaledStats;


    [Obsolete("No longer used"), JsonProperty("brain")]
    public string? Brain;

    [JsonProperty("forms")]
    public HeroForm[]? Forms;
}
