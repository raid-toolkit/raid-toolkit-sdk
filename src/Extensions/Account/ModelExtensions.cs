using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

using Raid.Toolkit.DataModel.Enums;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.DataModel
{
    public static partial class ModelExtensions
    {
        public static double AsDouble(this Plarium.Common.Numerics.Fixed num)
        {
            return (double)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
        }

        public static double? AsDouble(this Plarium.Common.Numerics.Fixed? num)
        {
            return num.HasValue ? num.Value.AsDouble() : null;
        }

        public static double SetStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind, double value)
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

        public static double AddStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind, double value)
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

        public static double GetStat(this Stats stats, SharedModel.Battle.Effects.StatKindId statKind)
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
                AwakenRank = hero.DoubleAscendData?.Grade.ToString(),
                Blessing = hero.DoubleAscendData?.BlessingId?.ToString(),
                FreeBlessingResetUsed = hero.DoubleAscendData?.FreeResetUsed == true ? true : null,
                Marker = hero.Marker.ToString(),
                Rank = hero.Grade.ToString(),
                Locked = hero.Locked,
                Deleted = false,
                InVault = hero.InStorage,
                InDeepVault = hero.InBathhouse,
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
                Health = stats.Health.AsDouble(),
                Attack = stats.Attack.AsDouble(),
                Defense = stats.Defence.AsDouble(),
                Accuracy = stats.Accuracy.AsDouble(),
                Resistance = stats.Resistance.AsDouble(),
                Speed = stats.Speed.AsDouble(),
                CriticalChance = stats.CriticalChance.AsDouble(),
                CriticalDamage = stats.CriticalDamage.AsDouble(),
                CriticalHeal = stats.CriticalHeal.AsDouble(),
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
                AscendLevel = artifact._ascendLevel,
                AscendBonus = artifact._ascendBonus?.ToModel(),
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
                Value = bonus._value._value.AsDouble(),
                GlyphPower = bonus._powerUpValue.AsDouble(),
                Level = bonus._level
            };
        }

        public static StatBonus ToModel(this SharedModel.Meta.General.StatBonus bonus)
        {
            return new()
            {
                Absolute = bonus.IsAbsolute,
                KindId = bonus.StatKindId.ToModel(),
                Value = bonus.Value.AsDouble(),
            };
        }

        public static StatBonus ToModel(this SharedModel.Meta.Artifacts.BonusValue bonus, SharedModel.Battle.Effects.StatKindId statKindId)
        {
            return new()
            {
                Absolute = bonus._isAbsolute,
                KindId = statKindId.ToModel(),
                Value = bonus._value.AsDouble(),
            };
        }

        public static ArtifactSetKind ToModel(this SharedModel.Meta.Artifacts.Sets.ArtifactSetInfo artifactSetInfo)
        {
            List<SharedModel.Meta.General.StatBonus> statBonuses = new();
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

        public static HeroVisualInfo ToModel(this SharedModel.Meta.Heroes.HeroVisualInfo visual)
        {
            return new()
            {
                AvatarName = visual.AvatarName,
                ModelName = visual.ModelName,
                ShowcaseSceneName = visual.ShowcaseSceneName,
            };
        }

        public static HeroForm ToModel(this SharedModel.Meta.Heroes.HeroForm form)
        {
            return new HeroForm()
            {
                Role = (HeroRole)form.Role,
                UnscaledStats = form.BaseStats.ToModel(),
                SkillTypeIds = form.SkillTypeIds?.ToArray(),
                VisualInfosBySkin = form.VisualInfosBySkinId?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel())
            };
        }

        public static HeroType ToModel(this SharedModel.Meta.Heroes.HeroType type)
        {
            SharedModel.Meta.Heroes.HeroVisualInfo? firstAvatar = type.VisualInfosBySkinId?.FirstOrDefault().Value;
            return new HeroType()
            {
                Affinity = (Enums.Element)type.Element,
                Ascended = type.Id % 10,
                AvatarKey = firstAvatar?.AvatarName,
                Faction = (Enums.HeroFraction)type.Fraction,
                ModelName = firstAvatar?.ModelName,
                Name = type.Name.ToModel(),
                ShortName = type.ShortName?.ToModel() ?? type.Name.ToModel(),
                Rarity = (Enums.HeroRarity)type.Rarity,
                Role = type._allRoles?.Cast<Enums.HeroRole>().FirstOrDefault(),
                LeaderSkill = type.LeaderSkill?.ToModel(),
                SkillTypeIds = type.AllSkillTypeIds?.ToArray(),
                TypeId = type.Id,
                UnscaledStats = type.BaseStats?.ToModel(),
                Forms = type.Forms.Select(ToModel).ToArray()
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
                Value = skill.Amount.AsDouble(),
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
                Unblockable = type.Unblockable,
                Upgrades = type.SkillLevelBonuses?.Select(bonus => bonus.ToModelObsolete()).ToArray(),
                SkillBonuses = type.SkillLevelBonuses?.Select(bonus => bonus.ToModel()).ToArray(),
                Visibility = type.Visibility.ToModel(),
                UseInTeamAttack = type.UseInTeamAttack,
                UseInCounterAttack = type.UseInCounterattack,
                Targets = type.Targets?.ToModel(),
                Group = type.Group.ToModel()
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

        public static EffectGroup ToModel(this SharedModel.Battle.Effects.EffectGroup type)
        {
            return (EffectGroup)type;
        }

        public static Visibility ToModel(this SharedModel.Meta.Skills.Visibility type)
        {
            return (Visibility)type;
        }

        public static EffectKindId ToModel(this SharedModel.Battle.Effects.EffectKindId kindId)
        {
            return (EffectKindId)kindId;
        }

        public static SkillTargets ToModel(this SharedModel.Meta.Skills.SkillTargets type)
        {
            return (SkillTargets)type;
        }

        public static SkillBonusType ToModel(this SharedModel.Meta.Skills.SkillBonusType type)
        {
            return (SkillBonusType)type;
        }

        public static SkillGroup ToModel(this SharedModel.Meta.Skills.SkillGroup type)
        {
            return (SkillGroup)type;
        }

        public static TargetParams ToModel(this SharedModel.Battle.Effects.EffectTargetParams.TargetParams type)
        {
            return new TargetParams
            {
                TargetType = (EffectTargetType)type.TargetType,
                Exclusion = (TargetExclusion?)type.Exclusion,
                Exclusive = type.Exclusive,
                FirstHitInSelected = type.FirstHitInSelected,
                Condition = type.Condition,
            };
        }

        public static EffectRelation ToModel(this SharedModel.Battle.Effects.EffectRelation type)
        {
            return new EffectRelation
            {
                EffectTypeId = type.EffectTypeId,
                EffectKindIds = type.EffectKindIds?.Cast<EffectKindId>().ToList(),
                EffectKindGroups = type.EffectKindGroups?.Cast<EffectKindGroup>().ToList(),
                Phases = type.Phases?.Cast<BattlePhaseId>().ToList(),
                ActivateOnGlancingHit = type.ActivateOnGlancingHit
            };
        }

        public static StatusEffectParams ToModel(this SharedModel.Battle.Effects.EffectParams.StatusEffectParams.StatusEffectParams type)
        {
            return new StatusEffectParams
            {
                StrengthInFamily = type.StrengthInFamily,
                ForcedTickAllowed = type.ForcedTickAllowed,
                LifetimeUpdateType = (LifetimeUpdateType)type.LifetimeUpdateType,
                UnapplyWhenProducerDied = type.UnapplyWhenProducerDied,
                SkipProcessingWhenJustApplied = type.SkipProcessingWhenJustApplied,
            };
        }

        public static ApplyStatusEffectParams ToModel(this SharedModel.Battle.Effects.EffectParams.ApplyStatusEffectParams type)
        {
            var stub = new ApplyStatusEffectParams
            {
                StatusEffectInfos = type.StatusEffectInfos.Select(entry => new StatusEffectInfo
                {
                    TypeId = entry.TypeId,
                    Duration = entry.Duration,
                    IgnoreEffectsLimit = entry.IgnoreEffectsLimit,
                    ApplyMode = (ApplyMode?)entry.ApplyMode,
                    Protection = entry.Protection?.ToModel(),
                    DurationFormula = entry.DurationFormula
                }).ToList()
            };
            return stub;
        }

        public static Protection ToModel(this SharedModel.Battle.Core.Skill.Protection protection)
        {
            return new Protection
            {
                Mode = (ProtectionMode)protection.Mode,
                Chance = protection.Chance.AsDouble()
            };
        }

        public static UnapplyStatusEffectParams ToModel(this SharedModel.Battle.Effects.EffectParams.UnapplyStatusEffectParams type)
        {
            return new UnapplyStatusEffectParams
            {
                Count = type.Count,
                StatusEffectTypeIds = type.StatusEffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                EffectKindGroups = type.EffectKindGroups?.Cast<StatusEffectTypeId>().ToList(),
                UnapplyMode = (UnapplyEffectMode)type.UnapplyMode,
                RemoveFrom = (UnapplyEffectTarget?)type.RemoveFrom,
                ApplyTo = (UnapplyEffectTarget?)type.ApplyTo,
            };
        }

        public static TransferDebuffParams ToModel(this SharedModel.Battle.Effects.EffectParams.TransferDebuffParams type)
        {
            return new TransferDebuffParams
            {
                Count = type.Count,
                StatusEffectTypeIds = type.StatusEffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                UnapplyMode = (UnapplyEffectMode)type.UnapplyMode,
                IncludeProducer = type.IncludeProducer,
                ApplyMode = (ApplyMode?)type.ApplyMode,
                RemoveFrom = (EffectTargetType)type.RemoveFrom,
                ApplyTo = (EffectTargetType)type.ApplyTo,
            };
        }

        public static DamageParams ToModel(this SharedModel.Battle.Effects.EffectParams.DamageParams.DamageParams type)
        {
            return new DamageParams
            {
                HitType = (HitType?)type.HitType,
                ElementRelation = (ElementRelation?)type.ElementRelation,
                DefenceModifier = type.DefenceModifier.AsDouble(),
                IsFixed = type.IsFixed,
                DoesNotCountAsHit = type.DoesNotCountAsHit,
                IncreaseCriticalHitChance = type.IncreaseCriticalHitChance.AsDouble(),
                IgnoreStatusEffectReduction = type.IgnoreStatusEffectReduction,
                ValueCapExpression = type.ValueCapExpression,
                SpecificDamageType = (SpecificDamageType?)type.SpecificDamageType
            };
        }

        public static HealParams ToModel(this SharedModel.Battle.Effects.EffectParams.HealParams type)
        {
            return new HealParams
            {
                CanBeCritical = type.CanBeCritical,
            };
        }

        public static EvenParams ToModel(this SharedModel.Battle.Effects.EffectParams.EvenParams type)
        {
            return new EvenParams
            {
                EvenMode = (EvenMode)type.EvenMode,
            };
        }

        public static ChangeStatParams ToModel(this SharedModel.Battle.Effects.EffectParams.ChangeStatParams type)
        {
            return new ChangeStatParams
            {
                Param = (Enums.StatKindId)type.Param,
            };
        }

        public static ActivateSkillParams ToModel(this SharedModel.Battle.Effects.EffectParams.ActivateSkillParams.ActivateSkillParams type)
        {
            return new ActivateSkillParams
            {
                SkillIndex = type.SkillIndex,
                SkillOwner = (ActivateSkillOwner)type.SkillOwner,
                TargetExpression = type.TargetExpression,
            };
        }

        public static ShowHiddenSkillParams ToModel(this SharedModel.Battle.Effects.EffectParams.ShowSecretSkillParams type)
        {
            return new ShowHiddenSkillParams
            {
                SkillTypeId = type.SkillTypeId,
                ShouldHide = type.ShouldHide,
            };
        }

        public static ChangeSkillCooldownParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeSkillCooldownParams type)
        {
            return new ChangeSkillCooldownParams
            {
                Turns = type.Turns,
                SkillIndex = type.SkillIndex,
                IsRandomSkill = type.IsRandomSkill,
                SkillToChange = (SkillToChange)type.SkillToChange,
            };
        }

        public static ChangeEffectLifetimeParams ToModel(this SharedModel.Battle.Effects.EffectParams.ChangeEffectLifetimeParams type)
        {
            return new ChangeEffectLifetimeParams
            {
                Type = (AppliedEffectType)type.Type,
                Turns = type.Turns,
                Count = type.Count,
                EffectTypeIds = type.EffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
            };
        }

        public static ShareDamageParams ToModel(this SharedModel.Battle.Effects.EffectParams.ShareDamageParams type)
        {
            return new ShareDamageParams
            {
                TargetDamageCutPerc = type.TargetDamageCutPerc.AsDouble(),
                TransferedDamagePerc = type.TransferedDamagePerc.AsDouble(),
                DefenceModifier = type.DefenceModifier.AsDouble(),
            };
        }

        public static BlockEffectParams ToModel(this SharedModel.Battle.Effects.EffectParams.BlockEffectParams type)
        {
            return new BlockEffectParams
            {
                EffectTypeIds = type.EffectTypeIds?.Cast<int>().ToList(),
                EffectKindIds = type.EffectKindIds?.Cast<int>().ToList(),
                BlockGuaranteed = type.BlockGuaranteed,
                BlockAllExcludeSelected = type.BlockAllExcludeSelected
            };
        }

        public static SummonParams ToModel(this SharedModel.Battle.Effects.EffectParams.SummonParams type)
        {
            return new SummonParams
            {
                BaseTypeId = type.BaseTypeId,
                AscendLevelFormula = type.AscendLevelFormula,
                GradeFormula = type.GradeFormula,
                LevelFormula = type.LevelFormula,
                RemoveAfterDeath = type.RemoveAfterDeath,
                SlotsLimit = type.SlotsLimit,
            };
        }

        public static TeamAttackParams ToModel(this SharedModel.Battle.Effects.EffectParams.TeamAttackParams type)
        {
            return new TeamAttackParams
            {
                TeammatesCount = type.TeammatesCount,
                ExcludeProducerFromAttack = type.ExcludeProducerFromAttack,
                PreferredHeroTypes = type.PreferredHeroTypes,
                AlwaysUsePreferredWhenPossible = type.AlwaysUsePreferredWhenPossible,
                AllySelectorExpression = type.AllySelectorExpression,
            };
        }

        public static DestroyHpParams ToModel(this SharedModel.Battle.Effects.EffectParams.DestroyHpParams type)
        {
            return new DestroyHpParams
            {
                IgnoreShield = type.IgnoreShield,
            };
        }

        public static ReviveParams ToModel(this SharedModel.Battle.Effects.EffectParams.ReviveParams type)
        {
            return new ReviveParams
            {
                HealPercent = type.HealPercent.AsDouble(),
                IgnoreBlockRevive = type.IgnoreBlockRevive,
            };
        }

        public static CounterattackParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.CounterattackParams type)
        {
            return new CounterattackParams
            {
                SkillIndex = type.SkillIndex,
                NoPenalty = type.NoPenalty,
            };
        }

        public static ForceStatusEffectTickParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ForceStatusEffectTickParams type)
        {
            return new ForceStatusEffectTickParams
            {
                Ticks = type.Ticks,
                EffectTypeIds = type.EffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                EffectCount = type.EffectCount,
            };
        }

        public static CrabShellLayer ToModel(
            this SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellLayer type)
        {
            return new CrabShellLayer
            {
                Type = (CrabShellLayerType)type.Type,
                MultiplierFormula = type.MultiplierFormula,
                ConditionFormula = type.ConditionFormula,
            };
        }

        public static IEnumerable<CrabShellLayer> ToModel(
            this IEnumerable<SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellLayer> type)
        {
            return type.Select(ToModel);
        }

        public static CrabShellParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellParams type)
        {
            return new CrabShellParams
            {
                Layers = type.Layers.ToModel().ToList(),
            };
        }

        public static ReturnDebuffsParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ReturnDebuffsParams type)
        {
            return new ReturnDebuffsParams
            {
                ApplyMode = (ApplyMode?)type.ApplyMode,
            };
        }

        public static HitTypeParams ToModel(this SharedModel.Battle.Effects.EffectParams.HitTypeParams type)
        {
            return new HitTypeParams
            {
                HitTypeToChange = (HitType?)type.HitTypeToChange,
                HitType = (HitType)type.HitType,
            };
        }

        public static PassiveBonusParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.PassiveBonusParams type)
        {
            return new PassiveBonusParams
            {
                Bonus = (PassiveBonus)type.Bonus,
            };
        }

        public static MultiplyStatusEffectParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.MultiplyStatusEffectParams type)
        {
            return new MultiplyStatusEffectParams
            {
                Count = type.Count,
                TurnsModifier = type.TurnsModifier,
                EffectKindIds = type.EffectKindIds?.Cast<EffectKindId>().ToList(),
                TargetSelectorExpression = type.TargetSelectorExpression,
                TurnsChangeMode = (TurnsChangeMode?)type.TurnsChangeMode
            };
        }

        public static IgnoreProtectionEffectsParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.IgnoreProtectionEffectsParams type)
        {
            return new IgnoreProtectionEffectsParams
            {
                IgnoreBlockDamage = type.IgnoreBlockDamage,
                IgnoreShield = type.IgnoreShield,
                IgnoreUnkillable = type.IgnoreUnkillable,
            };
        }

        public static ChangeEffectTargetParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeEffectTargetParams type)
        {
            return new ChangeEffectTargetParams
            {
                OverrideApplyMode = type.OverrideApplyMode,
                ApplyMode = (ApplyMode?)type.ApplyMode,
            };
        }

        public static PlaceHungerCounterParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.PlaceHungerCounterParams type)
        {
            return new PlaceHungerCounterParams
            {
                IterationsBetweenDevouring = type.IterationsBetweenDevouring,
            };
        }


        public static NewbieDefenceParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.NewbieDefenceParams type)
        {
            return new NewbieDefenceParams
            {
                ChangeDamageFactor = type.ChangeDamageFactor.AsDouble(),
            };
        }

        public static CocoonParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.CocoonParams type)
        {
            return new CocoonParams
            {
                StunTurns = type.StunTurns,
            };
        }

        public static PetrificationParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.PetrificationParams type)
        {
            return new PetrificationParams
            {
                GeneralChangeDamageFactor = type.GeneralChangeDamageFactor.AsDouble(),
                TimeBombChangeDamageFactor = type.TimeBombChangeDamageFactor.AsDouble(),
            };
        }

        public static StoneSkinParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.StoneSkinParams type)
        {
            return new StoneSkinParams
            {
                ReflectChance = type.ReflectChance.AsDouble(),
                TimeBombChangeDamageFactor = type.TimeBombChangeDamageFactor.AsDouble(),
                GeneralChangeDamageFactor = type.GeneralChangeDamageFactor.AsDouble(),
                PetrificationTurns = type.PetrificationTurns,
            };
        }

        public static DestroyStatsParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.DestroyStatsParams type)
        {
            return new DestroyStatsParams
            {
                StatKindId = (StatKindId)type.StatKindId,
                MaxDestructionPercentFormula = type.MaxDestructionPercentFormula,
            };
        }

        public static GrowHydraHeadParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.GrowHydraHeadParams type)
        {
            return new GrowHydraHeadParams
            {
                GrowSelfProbability = type.GrowSelfProbability.AsDouble(),
            };
        }

        public static DevourParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.DevourParams type)
        {
            return new DevourParams
            {
                DigestionLifetimeFormula = type.DigestionLifetimeFormula,
                DigestionStrengthFormula = type.DigestionStrengthFormula,
            };
        }

        public static TransformationParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.TransformationParams type)
        {
            return new TransformationParams
            {
                VariantId = type.VariantId,
            };
        }

        public static CancelTransformationParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.CancelTransformationParams.CancelTransformationParams type)
        {
            return new CancelTransformationParams
            {
                Stamina = type.Stamina.AsDouble(),
                HealthPercent = type.HealthPercent.AsDouble(),
            };
        }

        public static EffectContainerParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.EffectContainerParams type)
        {
            return new EffectContainerParams
            {
                Effect = type.Effect.ToModel(),
            };
        }

        public static ExcludeHitTypeParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ExcludeHitTypeParams type)
        {
            return new ExcludeHitTypeParams
            {
                ExcludeGlancingHit = type.ExcludeGlancingHit,
                ExcludeCriticalHit = type.ExcludeCriticalHit,
                ExcludeCrushingHit = type.ExcludeCrushingHit,
            };
        }

        public static ChangeShieldParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeShieldParams type)
        {
            return new ChangeShieldParams
            {
                ShieldTypes = type.ShieldTypes?.Cast<StatusEffectTypeId>().ToArray(),
            };
        }

        public static ChangeProtectionParams ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeProtectionParams type)
        {
            return new ChangeProtectionParams
            {
                Protection = type.Protection.ToModel(),
                CanReplaceStronger = type.CanReplaceStronger,
            };
        }

        public static EffectType ToModel(this SharedModel.Battle.Effects.EffectType type)
        {
            return new EffectType()
            {
                TypeId = type.Id,
                KindId = type.KindId.ToModel(),
                Count = type.Count,
                StackCount = type.StackCount,

                Group = type.Group.ToModel(),
                IsEffectDescription = type.IsEffectDescription,
                ConsidersDead = type.ConsidersDead,
                LeaveThroughDeath = type.LeaveThroughDeath,
                DoesntSetSkillOnCooldown = type.DoesntSetSkillOnCooldown,
                IgnoresCooldown = type.IgnoresCooldown,
                IsUnique = type.IsUnique,
                IterationChanceRolling = type.IterationChanceRolling,
                Relation = type.Relation?.ToModel(),
                Condition = type.Condition,
                Chance = type.Chance.AsDouble(),
                RepeatChance = type.RepeatChance.AsDouble(),
                //ValueCap = type.ValueCap,

                PlaceHungerCounterParams = type.PlaceHungerCounterParams?.ToModel(),
                NewbieDefenceParams = type.NewbieDefenceParams?.ToModel(),
                CocoonParams = type.CocoonParams?.ToModel(),
                PetrificationParams = type.PetrificationParams?.ToModel(),
                StoneSkinParams = type.StoneSkinParams?.ToModel(),
                ChangeEffectTargetParams = type.ChangeEffectTargetParams?.ToModel(),
                IgnoreProtectionEffectsParams = type.IgnoreProtectionEffectsParams?.ToModel(),
                MultiplyStatusEffectParams = type.MultiplyStatusEffectParams?.ToModel(),
                PassiveBonusParams = type.PassiveBonusParams?.ToModel(),
                HitTypeParams = type.HitTypeParams?.ToModel(),
                ReturnDebuffsParams = type.ReturnDebuffsParams?.ToModel(),
                CrabShellParams = type.CrabShellParams?.ToModel(),
                ForceTickParams = type.ForceTickParams?.ToModel(),
                CounterattackParams = type.CounterattackParams?.ToModel(),
                ReviveParams = type.ReviveParams?.ToModel(),
                DestroyStatsParams = type.DestroyStatsParams?.ToModel(),
                DestroyHpParams = type.DestroyHpParams?.ToModel(),
                TeamAttackParams = type.TeamAttackParams?.ToModel(),
                SummonParams = type.SummonParams?.ToModel(),
                BlockEffectParams = type.BlockEffectParams?.ToModel(),
                GrowHydraHeadParams = type.GrowHydraHeadParams?.ToModel(),
                DevourParams = type.DevourParams?.ToModel(),
                TransformationParams = type.TransformationParams?.ToModel(),
                CancelTransformationParams = type.CancelTransformationParams?.ToModel(),
                ShareDamageParams = type.ShareDamageParams?.ToModel(),
                EffectContainerParams = type.EffectContainerParams?.ToModel(),
                ExcludeHitTypeParams = type.ExcludeHitTypeParams?.ToModel(),
                ChangeShieldParams = type.ChangeShieldParams?.ToModel(),
                ChangeProtectionParams = type.ChangeProtectionParams?.ToModel(),
                ChangeEffectLifetimeParams = type.ChangeEffectLifetimeParams?.ToModel(),
                ChangeSkillCooldownParams = type.ChangeSkillCooldownParams?.ToModel(),
                ShowHiddenSkillParams = type.ShowSecretSkillParams?.ToModel(),
                TargetParams = type.TargetParams.ToModel(),
                StatusParams = type.StatusParams?.ToModel(),
                ChangeStatParams = type.ChangeStatParams?.ToModel(),
                EvenParams = type.EvenParams?.ToModel(),
                HealParams = type.HealParams?.ToModel(),
                DamageParams = type.DamageParams?.ToModel(),
                TransferDebuffParams = type.TransferDebuffParams?.ToModel(),
                UnapplyStatusEffectParams = type.UnapplyStatusEffectParams?.ToModel(),
                ActivateSkillParams = type.ActivateSkillParams?.ToModel(),
                ApplyStatusEffectParams = type.ApplyStatusEffectParams?.ToModel(),

                IsContainer = type.IsContainer,
                ApplyInstantEffectMode = (ApplyMode?)type.ApplyInstantEffectMode,
                PersistsThroughRounds = type.PersistsThroughRounds,
                SnapshotRequired = type.SnapshotRequired,
                IgnoredEffects = type.IgnoredEffects?.Cast<EffectKindId>().ToList(),
                MultiplierFormula = type.MultiplierFormula,
                MultiplierNotEvaluatedByAI = type.MultiplierNotEvaluatedByAI,
            };
        }

        public static SkillUpgrade ToModelObsolete(this SharedModel.Meta.Skills.SkillBonus bonus)
        {
            return new SkillUpgrade()
            {
                SkillBonusType = bonus.SkillBonusType.ToString(),
                Value = bonus.Value.AsDouble(),
            };
        }

        public static SkillBonus ToModel(this SharedModel.Meta.Skills.SkillBonus bonus)
        {
            return new SkillBonus()
            {
                SkillBonusType = bonus.SkillBonusType.ToModel(),
                Value = bonus.Value.AsDouble(),
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
                AreaId = (int)areaTypeId,
                RegionId = (int)regionTypeId,
                Difficulty = stage._difficulty?.ToString(),
                StageId = stage.Id,
                // too slow
                //Modifiers = stage.Modifiers.Select(ToModel).ToArray(),
                //Formations = stage.Formations.Select(ToModel).ToArray()
            };
        }

        public static StatsModifier ToModel(this SharedModel.Meta.Stages.BattleStatsModifier battleStatsMod)
        {
            return new()
            {
                Round = battleStatsMod.Round,
                BossOnly = battleStatsMod.BossOnly,
                Value = battleStatsMod.Value,
                Absolute = battleStatsMod.IsAbsolute,
                KindId = battleStatsMod.KindId.ToModel()
            };
        }

        public static StageFormation ToModel(this SharedModel.Meta.Stages.StageHeroesFormation formation)
        {
            return new()
            {
                Id = formation.Id,
                HeroSetups = formation.HeroSlotsSetup?.Select(ToModel).ToArray()
            };
        }

        public static HeroSlotSetup ToModel(this SharedModel.Meta.Stages.ShortHeroSlotSetup heroSlotSetup)
        {
            return new()
            {
                TypeId = heroSlotSetup.HeroTypeId,
                Round = heroSlotSetup.Round,
                Slot = heroSlotSetup.Slot,
                Level = heroSlotSetup.Level,
                Grade = (HeroGrade)heroSlotSetup.Grade,
                Awaken = ((int?)heroSlotSetup.DoubleAscendGrade ?? 0),
                MaxSkillsLevel = heroSlotSetup.MaxSkillsLevel,
                Modifiers = heroSlotSetup.Modifiers?.ToModel(),
            };
        }

        public static StatsInitialState? ToModel(this SharedModel.Battle.Core.Setup.InitialState? initialState)
        {
            if (initialState == null)
                return null;

            return new()
            {
                DamageTaken = initialState.DamageTaken.AsDouble(),
                HealthModifier = initialState.HealthModifier.AsDouble()
            };
        }
        public static StatsModifierSetup ToModel(this SharedModel.Meta.Stages.BattleStatsModifierSetup? statsModifierSetup)
        {
            if (statsModifierSetup == null)
                return null;

            return new()
            {
                FlatBonus = statsModifierSetup.FlatBonus?.ToModel(),
                PercentBonus = statsModifierSetup.PercentBonus?.ToModel(),
                InitialState = statsModifierSetup.InitialState.ToModel()
            };
        }
    }
}
