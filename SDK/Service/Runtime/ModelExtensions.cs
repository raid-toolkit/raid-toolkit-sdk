using System;
using System.Collections.Generic;
using System.Linq;
using SharedModel.Meta.Artifacts;

namespace Raid.DataModel
{
    public static partial class ModelExtensions
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num)
        {
            return (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
        }

        public static IReadOnlyDictionary<string, V> ToModel<K, V>(this IDictionary<K, V> dict) where K : Enum
        {
            return dict == null ? new Dictionary<string, V>() : dict.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }

        public static IReadOnlyDictionary<K, V> ToModelEnum<K, V>(this IDictionary<K, V> dict) where K : Enum
        {
            return dict == null ? new Dictionary<K, V>() : dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static float SetStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind, float value)
        {
            switch (statKind)
            {
                case SharedModel.Battle.Effects.StatKindId.Accuracy:
                    return stats.Accuracy = value;
                case SharedModel.Battle.Effects.StatKindId.Attack:
                    return stats.Attack = value;
                case SharedModel.Battle.Effects.StatKindId.CriticalChance:
                    return stats.CriticalChance = value;
                case SharedModel.Battle.Effects.StatKindId.CriticalDamage:
                    return stats.CriticalDamage = value;
                case SharedModel.Battle.Effects.StatKindId.CriticalHeal:
                    return stats.CriticalHeal = value;
                case SharedModel.Battle.Effects.StatKindId.Defence:
                    return stats.Defense = value;
                case SharedModel.Battle.Effects.StatKindId.Health:
                    return stats.Health = value;
                case SharedModel.Battle.Effects.StatKindId.Resistance:
                    return stats.Resistance = value;
                case SharedModel.Battle.Effects.StatKindId.Speed:
                    return stats.Speed = value;
                default:
                    break;
            }
            return value;
        }

        public static float AddStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind, float value)
        {
            switch (statKind)
            {
                case SharedModel.Battle.Effects.StatKindId.Accuracy:
                    return stats.Accuracy += value;
                case SharedModel.Battle.Effects.StatKindId.Attack:
                    return stats.Attack += value;
                case SharedModel.Battle.Effects.StatKindId.CriticalChance:
                    return stats.CriticalChance += value;
                case SharedModel.Battle.Effects.StatKindId.CriticalDamage:
                    return stats.CriticalDamage += value;
                case SharedModel.Battle.Effects.StatKindId.CriticalHeal:
                    return stats.CriticalHeal += value;
                case SharedModel.Battle.Effects.StatKindId.Defence:
                    return stats.Defense += value;
                case SharedModel.Battle.Effects.StatKindId.Health:
                    return stats.Health += value;
                case SharedModel.Battle.Effects.StatKindId.Resistance:
                    return stats.Resistance += value;
                case SharedModel.Battle.Effects.StatKindId.Speed:
                    return stats.Speed += value;
                default:
                    break;
            }
            return value;
        }

        public static float GetStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind)
        {
            switch (statKind)
            {
                case SharedModel.Battle.Effects.StatKindId.Accuracy:
                    return stats.Accuracy;
                case SharedModel.Battle.Effects.StatKindId.Attack:
                    return stats.Attack;
                case SharedModel.Battle.Effects.StatKindId.CriticalChance:
                    return stats.CriticalChance;
                case SharedModel.Battle.Effects.StatKindId.CriticalDamage:
                    return stats.CriticalDamage;
                case SharedModel.Battle.Effects.StatKindId.CriticalHeal:
                    return stats.CriticalHeal;
                case SharedModel.Battle.Effects.StatKindId.Defence:
                    return stats.Defense;
                case SharedModel.Battle.Effects.StatKindId.Health:
                    return stats.Health;
                case SharedModel.Battle.Effects.StatKindId.Resistance:
                    return stats.Resistance;
                case SharedModel.Battle.Effects.StatKindId.Speed:
                    return stats.Speed;
                default:
                    break;
            }
            return 0f;
        }

        public static LocalizedText ToModel(this SharedModel.Common.Localization.SharedLTextKey key)
        {
            string localizedValue = StaticResources.LocalizeByKey(key.Key);
            return new()
            {
                Key = key.Key,
                DefaultValue = key.DefaultValue,
                LocalizedValue = localizedValue
            };
        }

        public static Hero ToModel(
            this SharedModel.Meta.Heroes.Hero hero,
            Dictionary<ArtifactKindId, int> equippedArtifacts,
            HeroType heroType)
        {
            Hero result = new()
            {
                Id = hero.Id,
                TypeId = hero.TypeId,
                Level = hero.Level,
                EmpowerLevel = hero.EmpowerLevel,
                Marker = hero.Marker.ToString(),
                Rank = hero.Grade.ToString(),
                Locked = hero.Locked,
                Deleted = false,
                InVault = hero.InStorage,
                Experience = hero.Experience,
                FullExperience = hero.FullExperience,
                Masteries = hero.MasteryData?.Masteries.Cast<MasteryKindId>().ToArray() ?? Array.Empty<MasteryKindId>(),
                AssignedMasteryScrolls = hero.MasteryData?.TotalAmount.UnderlyingDictionary.ToModel() ?? new Dictionary<string, int>(),
                UnassignedMasteryScrolls = hero.MasteryData?.CurrentAmount.UnderlyingDictionary.ToModel() ?? new Dictionary<string, int>(),
                TotalMasteryScrolls = new Dictionary<string, int>(),
                EquippedArtifactIds = equippedArtifacts.ToModel(),
                Type = heroType,
                Name = heroType.Name.Localize(),
#pragma warning disable 0618
                SkillLevelsByTypeId = hero.Skills.ToDictionary(skill => skill.TypeId, skill => skill.Level),
#pragma warning restore 0618
                SkillsById = hero.Skills.ToDictionary(skill => skill.Id, skill => skill.ToModel()),
            };

            result.TotalMasteryScrolls = result.AssignedMasteryScrolls
                .Concat(result.UnassignedMasteryScrolls)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.Value))
            ;
            return result;
        }

        public static Stats ToModel(this SharedModel.Meta.Heroes.BattleStats stats)
        {
            return new()
            {
                Health = stats.Health.AsFloat(),
                Attack = stats.Attack.AsFloat(),
                Defense = stats.Defence.AsFloat(),
                Accuracy = stats.Accuracy.AsFloat(),
                Resistance = stats.Resistance.AsFloat(),
                Speed = stats.Speed.AsFloat(),
                CriticalChance = stats.CriticalChance.AsFloat(),
                CriticalDamage = stats.CriticalDamage.AsFloat(),
                CriticalHeal = stats.CriticalHeal.AsFloat(),
            };
        }

        public static ArenaLeague ToModel(this SharedModel.Meta.Battle.Arena.ArenaLeagueInfo arenaLeague)
        {
            return new()
            {
                Id = arenaLeague.Id.ToString(),
                StatBonus = arenaLeague.BattleBonuses.ToModel(),
            };
        }

        public static string ToModel(this SharedModel.Battle.Effects.StatKindId statKind)
        {
            return statKind == SharedModel.Battle.Effects.StatKindId.Defence ? "Defense" : statKind.ToString();
        }

        public static Artifact ToModel(this SharedModel.Meta.Artifacts.Artifact artifact)
        {
            return new()
            {
                Id = artifact._id,
                KindId = artifact._kindId.ToString(),
                SetKindId = artifact._setKindId.ToString(),
                Level = artifact._level,
                Rank = artifact._rankId.ToString(),
                RarityId = artifact._rarityId.ToString(),
                Seen = artifact._isSeen,
                Activated = artifact._isActivated,
                SellPrice = artifact._sellPrice,
                Price = artifact._price,
                Faction = artifact._requiredFraction.ToString(),
                FailedUpgrades = artifact._failedUpgrades,
                Revision = artifact._revision,
                PrimaryBonus = artifact._primaryBonus.ToModel(),
                SecondaryBonuses = artifact._secondaryBonuses.Select(bonus => bonus.ToModel()).ToList()
            };
        }

        public static ArtifactStatBonus ToModel(this SharedModel.Meta.Artifacts.Bonuses.ArtifactBonus bonus)
        {
            return new()
            {
                KindId = bonus._kindId.ToModel(),
                Absolute = bonus._value._isAbsolute,
                Value = bonus._value._value.AsFloat(),
                GlyphPower = bonus._powerUpValue.AsFloat(),
                Level = bonus._level
            };
        }

        public static StatBonus ToModel(this SharedModel.Meta.Artifacts.Sets.ArtifactSetStatBonus bonus)
        {
            return new()
            {
                Absolute = bonus.IsAbsolute,
                KindId = bonus.StatKindId.ToModel(),
                Value = bonus.Value.AsFloat(),
            };
        }

        public static StatBonus ToModel(this BonusValue bonus, SharedModel.Battle.Effects.StatKindId statKindId)
        {
            return new()
            {
                Absolute = bonus._isAbsolute,
                KindId = statKindId.ToModel(),
                Value = bonus._value.AsFloat(),
            };
        }

        public static ArtifactSetKind ToModel(this SharedModel.Meta.Artifacts.Sets.ArtifactSetInfo artifactSetInfo)
        {
            List<SharedModel.Meta.Artifacts.Sets.ArtifactSetStatBonus> statBonuses = new();
            if (artifactSetInfo.StatBonus != null) statBonuses.Add(artifactSetInfo.StatBonus);
            if (artifactSetInfo.StatBonuses != null) statBonuses.AddRange(artifactSetInfo.StatBonuses);

            return new()
            {
                SetKindId = artifactSetInfo.ArtifactSetKindId.ToString(),
                ArtifactCount = artifactSetInfo.ArtifactCount,
                Name = artifactSetInfo.Name.ToModel(),
                SkillBonus = artifactSetInfo.SkillBonus?.SkillTypeId,
                StatBonuses = statBonuses.Select(bonus => bonus.ToModel()).ToArray(),
                LongDescription = artifactSetInfo.Description.ToModel(),
                ShortDescription = artifactSetInfo.ShortDescription.ToModel(),
            };
        }

        public static HeroType ToModel(this SharedModel.Meta.Heroes.HeroType type)
        {
            return new HeroType()
            {
                Affinity = type.Element.ToString(),
                Ascended = type.Id % 10,
                AvatarKey = type.AvatarName,
                Faction = type.Fraction.ToString(),
                ModelName = type.ModelName,
                Name = type.Name.ToModel(),
                Rarity = type.Rarity.ToString(),
                Role = type.Role.ToString(),
                LeaderSkill = type.LeaderSkill?.ToModel(),
                SkillTypeIds = type.SkillTypeIds?.ToArray(),
                TypeId = type.Id,
                UnscaledStats = type.BaseStats.ToModel(),
                Brain = type.Brain.ToString(),
            };
        }

        public static LeaderStatBonus ToModel(this SharedModel.Meta.Skills.LeaderSkill skill)
        {
            return new()
            {
                Absolute = skill.IsAbsolute,
                Affinity = skill.Element.ToString(),
                AreaTypeId = skill.Area.ToString(),
                KindId = skill.StatKindId.ToString(),
                Value = skill.Amount.AsFloat(),
            };
        }

        public static SkillType ToModel(this SharedModel.Meta.Skills.SkillType type)
        {
            return new SkillType()
            {
                TypeId = type.Id,
                Cooldown = type.Cooldown,
                Description = type.Description.ToModel(),
                Effects = type.Effects.Select(effect => effect.ToModel()).ToArray(),
                Name = type.Name.ToModel(),
                Unblockable = type.Unblockable.ToNullable(),
                Upgrades = type.SkillLevelBonuses?.Select(bonus => bonus.ToModel()).ToArray(),
                Visibility = type.Visibility.ToString(),
            };
        }

        public static Skill ToModel(this SharedModel.Meta.Skills.Skill skill)
        {
            return new Skill()
            {
                Id = skill.Id,
                TypeId = skill.TypeId,
                Level = skill.Level,
            };
        }

        public static EffectType ToModel(this SharedModel.Battle.Effects.EffectType type)
        {
            return new EffectType()
            {
                TypeId = type.Id,
                KindId = type.KindId.ToString(),
                Count = type.Count,
                StackCount = type.StackCount,
                Multiplier = type.MultiplierFormula,
            };
        }

        public static SkillUpgrade ToModel(this SharedModel.Meta.Skills.SkillBonus bonus)
        {
            return new SkillUpgrade()
            {
                SkillBonusType = bonus.SkillBonusType.ToString(),
                Value = bonus.Value.AsFloat(),
            };
        }

        public static AreaData ToModel(this SharedModel.Meta.Stages.Area area)
        {
            return new()
            {
                Name = area.Name.ToModel(),
                AreaId = (int)area.Id
            };
        }

        public static RegionData ToModel(this SharedModel.Meta.Stages.Region region, SharedModel.Meta.Stages.AreaTypeId areaTypeId)
        {
            return new()
            {
                Name = region.Name.ToModel(),
                RegionId = (int)region.Id,
                AreaId = (int)areaTypeId
            };
        }

        public static StageData ToModel(this SharedModel.Meta.Stages.Stage stage, SharedModel.Meta.Stages.AreaTypeId areaTypeId, SharedModel.Meta.Stages.RegionTypeId regionTypeId)
        {
            return new()
            {
                Name = stage.Name.ToModel(),
                AreaId = (int)areaTypeId,
                RegionId = (int)regionTypeId,
                Difficulty = stage._difficulty.ToString(),
                StageId = stage.Id,
            };
        }
    }
}
