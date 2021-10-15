using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SharedModel.Meta.Artifacts.Sets;

namespace Raid.Service.DataModel
{
    public class ArtifactSetKind
    {
        [JsonProperty("setKindId")]
        public ArtifactSetKindId SetKindId;

        [JsonProperty("artifactCount")]
        public int ArtifactCount;

        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("statBonuses")]
        public StatBonus[] StatBonuses;

        [JsonProperty("skillBonus")]
        public int? SkillBonus;

        [JsonProperty("shortDescription")]
        public LocalizedText ShortDescription;

        [JsonProperty("longDescription")]
        public LocalizedText LongDescription;
    }

    public static partial class ModelExtensions
    {
        public static ArtifactSetKind ToModel(this ArtifactSetInfo artifactSetInfo)
        {
            List<ArtifactSetStatBonus> statBonuses = new();
            if (artifactSetInfo.StatBonus != null) statBonuses.Add(artifactSetInfo.StatBonus);
            if (artifactSetInfo.StatBonuses != null) statBonuses.AddRange(artifactSetInfo.StatBonuses);

            return new()
            {
                SetKindId = artifactSetInfo.ArtifactSetKindId,
                ArtifactCount = artifactSetInfo.ArtifactCount,
                Name = artifactSetInfo.Name.ToModel(),
                SkillBonus = artifactSetInfo.SkillBonus?.SkillTypeId,
                StatBonuses = statBonuses.Select(bonus => bonus.ToModel()).ToArray(),
                LongDescription = artifactSetInfo.Description.ToModel(),
                ShortDescription = artifactSetInfo.ShortDescription.ToModel(),
            };
        }
    }
}