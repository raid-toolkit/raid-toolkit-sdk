using System;
using System.Collections.Generic;
using System.Linq;
using SharedModel.Meta.Artifacts;

namespace Raid.DataModel
{
    public static partial class ModelExtensions
    {
        public static double AsDouble(this Plarium.Common.Numerics.Fixed num)
        {
            return (double)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
        }

        public static double? AsDouble(this Il2CppToolkit.Runtime.Types.corelib.Native__Nullable<Plarium.Common.Numerics.Fixed> num)
        {
            return num.HasValue ? num.Value.AsDouble() : null;
        }

        public static IReadOnlyDictionary<string, V> ToModel<K, V>(this IDictionary<K, V> dict) where K : Enum
        {
            return dict == null ? new Dictionary<string, V>() : dict.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }

        public static IReadOnlyDictionary<K, V> ToModelEnum<K, V>(this IDictionary<K, V> dict) where K : Enum
        {
            return dict == null ? new Dictionary<K, V>() : dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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

        public static StatBonus ToModel(this SharedModel.Meta.Artifacts.Sets.ArtifactSetStatBonus bonus)
        {
            return new()
            {
                Absolute = bonus.IsAbsolute,
                KindId = bonus.StatKindId.ToModel(),
                Value = bonus.Value.AsDouble(),
            };
        }

        public static StatBonus ToModel(this BonusValue bonus, SharedModel.Battle.Effects.StatKindId statKindId)
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
                Affinity = (Enums.Element)type.Element,
                Ascended = type.Id % 10,
                AvatarKey = type.AvatarName,
                Faction = (Enums.HeroFraction)type.Fraction,
                ModelName = type.ModelName,
                Name = type.Name.ToModel(),
                Rarity = (Enums.HeroRarity)type.Rarity,
                Role = (Enums.HeroRole)type.Role,
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
                Unblockable = type.Unblockable.ToNullable(),
                Upgrades = type.SkillLevelBonuses?.Select(bonus => bonus.ToModel()).ToArray(),
                Visibility = type.Visibility.ToModel(),
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

        public static TargetParamsStub ToModel(this SharedModel.Battle.Effects.EffectTargetParams.TargetParams type)
        {
            return new TargetParamsStub
            {
                TargetType = (EffectTargetType)type.TargetType,
                Exclusion = (TargetExclusion)type.Exclusion.Value,
                Exclusive = type.Exclusive,
                FirstHitInSelected = type.FirstHitInSelected,
                Condition = type.Condition,
            };
        }

        public static EffectRelationStub ToModel(this SharedModel.Battle.Effects.EffectRelation type)
        {
            return new EffectRelationStub
            {
                EffectTypeId = type.EffectTypeId.Value,
                EffectKindIds = type.EffectKindIds?.Cast<EffectKindId>().ToList(),
                EffectKindGroups = type.EffectKindGroups?.Cast<EffectKindGroup>().ToList(),
                Phases = type.Phases?.Cast<BattlePhaseId>().ToList(),
                ActivateOnGlancingHit = type.ActivateOnGlancingHit
            };
        }

        public static StatusEffectParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.StatusEffectParams.StatusEffectParams type)
        {
            return new StatusEffectParamsStub
            {
                StrengthInFamily = type.StrengthInFamily,
                ForcedTickAllowed = type.ForcedTickAllowed,
                LifetimeUpdateType = (LifetimeUpdateType)type.LifetimeUpdateType,
                UnapplyWhenProducerDied = type.UnapplyWhenProducerDied.Value,
            };
        }

        public static ApplyStatusEffectParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ApplyStatusEffectParams type)
        {
            var stub = new ApplyStatusEffectParamsStub
            {
                StatusEffectInfos = type.StatusEffectInfos.Select(entry => new StatusEffectInfoStub
                {
                    TypeId = entry.TypeId,
                    Duration = entry.Duration,
                    IgnoreEffectsLimit = entry.IgnoreEffectsLimit,
                    ApplyMode = (ApplyMode)entry.ApplyMode.Value,
                    Protection = entry.Protection?.ToModel()
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

        public static UnapplyStatusEffectParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.UnapplyStatusEffectParams type)
        {
            return new UnapplyStatusEffectParamsStub
            {
                Count = type.Count,
                StatusEffectTypeIds = type.StatusEffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                UnapplyMode = (UnapplyEffectMode)type.UnapplyMode,
                RemoveFrom = (UnapplyEffectTarget)type.RemoveFrom.Value,
                ApplyTo = (UnapplyEffectTarget)type.ApplyTo.Value,
            };
        }

        public static TransferDebuffParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.TransferDebuffParams type)
        {
            return new TransferDebuffParamsStub
            {
                Count = type.Count,
                StatusEffectTypeIds = type.StatusEffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                UnapplyMode = (UnapplyEffectMode)type.UnapplyMode,
                IncludeProducer = type.IncludeProducer,
                ApplyMode = (ApplyMode)type.ApplyMode.Value,
                RemoveFrom = (EffectTargetType)type.RemoveFrom,
                ApplyTo = (EffectTargetType)type.ApplyTo,
            };
        }

        public static DamageParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.DamageParams type)
        {
            return new DamageParamsStub
            {
                HitType = (HitType)type.HitType.Value,
                ElementRelation = (ElementRelation)type.ElementRelation.Value,
                DefenceModifier = type.DefenceModifier.AsDouble(),
                IsFixed = type.IsFixed,
                DoesNotCountAsHit = type.DoesNotCountAsHit,
                IncreaseCriticalHitChance = type.IncreaseCriticalHitChance.AsDouble(),
                ValueCapExpression = type.ValueCapExpression,
            };
        }

        public static HealParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.HealParams type)
        {
            return new HealParamsStub
            {
                CanBeCritical = type.CanBeCritical,
            };
        }

        public static EvenParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.EvenParams type)
        {
            return new EvenParamsStub
            {
                EvenMode = (EvenMode)type.EvenMode,
            };
        }

        public static ChangeStatParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ChangeStatParams type)
        {
            return new ChangeStatParamsStub
            {
                Param = (Enums.StatKindId)type.Param,
            };
        }

        public static ActivateSkillParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ActivateSkillParams.ActivateSkillParams type)
        {
            return new ActivateSkillParamsStub
            {
                SkillIndex = type.SkillIndex,
                SkillOwner = (ActivateSkillOwner)type.SkillOwner,
                TargetExpression = type.TargetExpression,
            };
        }

        public static ShowHiddenSkillParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ShowHiddenSkillParams type)
        {
            return new ShowHiddenSkillParamsStub
            {
                SkillTypeId = type.SkillTypeId,
                ShouldHide = type.ShouldHide,
            };
        }

        public static ChangeSkillCooldownParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeSkillCooldownParams type)
        {
            return new ChangeSkillCooldownParamsStub
            {
                Turns = type.Turns,
                SkillIndex = type.SkillIndex.Value,
                IsRandomSkill = type.IsRandomSkill.Value,
                SkillToChange = (SkillToChange)type.SkillToChange,
            };
        }

        public static ChangeEffectLifetimeParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ChangeEffectLifetimeParams type)
        {
            return new ChangeEffectLifetimeParamsStub
            {
                Type = (AppliedEffectType)type.Type,
                Turns = type.Turns,
                Count = type.Count,
                EffectTypeIds = type.EffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
            };
        }

        public static ShareDamageParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ShareDamageParams type)
        {
            return new ShareDamageParamsStub
            {
                TargetDamageCutPerc = type.TargetDamageCutPerc.AsDouble(),
                TransferedDamagePerc = type.TransferedDamagePerc.AsDouble(),
                DefenceModifier = type.DefenceModifier.AsDouble(),
            };
        }

        public static BlockEffectParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.BlockEffectParams type)
        {
            return new BlockEffectParamsStub
            {
                EffectTypeIds = type.EffectTypeIds?.Cast<int>().ToList(),
                EffectKindIds = type.EffectKindIds?.Cast<int>().ToList(),
                BlockGuaranteed = type.BlockGuaranteed.Value,
            };
        }

        public static SummonParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.SummonParams type)
        {
            return new SummonParamsStub
            {
                BaseTypeId = type.BaseTypeId,
                AscendLevelFormula = type.AscendLevelFormula,
                GradeFormula = type.GradeFormula,
                LevelFormula = type.LevelFormula,
                RemoveAfterDeath = type.RemoveAfterDeath,
                SlotsLimit = type.SlotsLimit,
            };
        }

        public static TeamAttackParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.TeamAttackParams type)
        {
            return new TeamAttackParamsStub
            {
                TeammatesCount = type.TeammatesCount,
                ExcludeProducerFromAttack = type.ExcludeProducerFromAttack,
                PreferredHeroTypes = type.PreferredHeroTypes,
                AlwaysUsePreferredWhenPossible = type.AlwaysUsePreferredWhenPossible.Value,
                AllySelectorExpression = type.AllySelectorExpression,
            };
        }

        public static DestroyHpParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.DestroyHpParams type)
        {
            return new DestroyHpParamsStub
            {
                IgnoreShield = type.IgnoreShield,
            };
        }

        public static ReviveParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.ReviveParams type)
        {
            return new ReviveParamsStub
            {
                HealPercent = type.HealPercent.AsDouble(),
                IgnoreBlockRevive = type.IgnoreBlockRevive,
            };
        }

        public static CounterattackParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.CounterattackParams type)
        {
            return new CounterattackParamsStub
            {
                SkillIndex = type.SkillIndex,
                NoPenalty = type.NoPenalty,
            };
        }

        public static ForceStatusEffectTickParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.ForceStatusEffectTickParams type)
        {
            return new ForceStatusEffectTickParamsStub
            {
                Ticks = type.Ticks,
                EffectTypeIds = type.EffectTypeIds?.Cast<StatusEffectTypeId>().ToList(),
                EffectCount = type.EffectCount,
            };
        }

        public static CrabShellLayerStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellLayer type)
        {
            return new CrabShellLayerStub
            {
                Type = (CrabShellLayerType)type.Type,
                MultiplierFormula = type.MultiplierFormula,
                ConditionFormula = type.ConditionFormula,
            };
        }

        public static IEnumerable<CrabShellLayerStub> ToModel(
            this IEnumerable<SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellLayer> type)
        {
            return type.Select(ToModel);
        }

        public static CrabShellParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.CrabShellParams.CrabShellParams type)
        {
            return new CrabShellParamsStub
            {
                Layers = type.Layers.ToModel().ToList(),
            };
        }

        public static ReturnDebuffsParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.ReturnDebuffsParams type)
        {
            return new ReturnDebuffsParamsStub
            {
                ApplyMode = (ApplyMode)type.ApplyMode.Value,
            };
        }

        public static HitTypeParamsStub ToModel(this SharedModel.Battle.Effects.EffectParams.HitTypeParams type)
        {
            return new HitTypeParamsStub
            {
                HitTypeToChange = (HitType)type.HitTypeToChange.Value,
                HitType = (HitType)type.HitType,
            };
        }

        public static PassiveBonusParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.PassiveBonusParams type)
        {
            return new PassiveBonusParamsStub
            {
                Bonus = (PassiveBonus)type.Bonus,
            };
        }

        public static MultiplyStatusEffectParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.MultiplyStatusEffectParams type)
        {
            return new MultiplyStatusEffectParamsStub
            {
                Count = type.Count,
                TurnsModifier = type.TurnsModifier,
                EffectKindIds = type.EffectKindIds?.Cast<EffectKindId>().ToList(),
                TargetSelectorExpression = type.TargetSelectorExpression,
            };
        }

        public static IgnoreProtectionEffectsParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.IgnoreProtectionEffectsParams type)
        {
            return new IgnoreProtectionEffectsParamsStub
            {
                IgnoreBlockDamage = type.IgnoreBlockDamage,
                IgnoreShield = type.IgnoreShield,
                IgnoreUnkillable = type.IgnoreUnkillable,
            };
        }

        public static ChangeEffectTargetParamsStub ToModel(
            this SharedModel.Battle.Effects.EffectParams.ChangeEffectTargetParams type)
        {
            return new ChangeEffectTargetParamsStub
            {
                OverrideApplyMode = type.OverrideApplyMode,
                ApplyMode = (ApplyMode)type.ApplyMode.Value,
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
                Multiplier = type.MultiplierFormula,

                Group = type.Group.ToModel(),
                TargetParams = type.TargetParams.ToModel(),
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
                StatusParams = type.StatusParams?.ToModel(),
                ValueCap = type.ValueCap,
                ApplyInstantEffectMode = (ApplyMode)type.ApplyInstantEffectMode.Value,
                PersistsThroughRounds = type.PersistsThroughRounds,
                SnapshotRequired = type.SnapshotRequired,
                IgnoredEffects = type.IgnoredEffects?.Cast<EffectKindId>().ToList(),
                ApplyStatusEffectParams = type.ApplyStatusEffectParams?.ToModel(),
                UnapplyStatusEffectParams = type.UnapplyStatusEffectParams?.ToModel(),
                TransferDebuffParams = type.TransferDebuffParams?.ToModel(),
                DamageParams = type.DamageParams?.ToModel(),
                HealParams = type.HealParams?.ToModel(),
                EvenParams = type.EvenParams?.ToModel(),
                ChangeStatParams = type.ChangeStatParams?.ToModel(),
                ActivateSkillParams = type.ActivateSkillParams?.ToModel(),
                ShowHiddenSkillParams = type.ShowHiddenSkillParams?.ToModel(),
                ChangeSkillCooldownParams = type.ChangeSkillCooldownParams?.ToModel(),
                ChangeEffectLifetimeParams = type.ChangeEffectLifetimeParams?.ToModel(),
                ShareDamageParams = type.ShareDamageParams?.ToModel(),
                BlockEffectParams = type.BlockEffectParams?.ToModel(),
                SummonParams = type.SummonParams?.ToModel(),
                TeamAttackParams = type.TeamAttackParams?.ToModel(),
                DestroyHpParams = type.DestroyHpParams?.ToModel(),
                ReviveParams = type.ReviveParams?.ToModel(),
                CounterattackParams = type.CounterattackParams?.ToModel(),
                ForceTickParams = type.ForceTickParams?.ToModel(),
                CrabShellParams = type.CrabShellParams?.ToModel(),
                ReturnDebuffsParams = type.ReturnDebuffsParams?.ToModel(),
                HitTypeParams = type.HitTypeParams?.ToModel(),
                PassiveBonusParams = type.PassiveBonusParams?.ToModel(),
                MultiplyStatusEffectParams = type.MultiplyStatusEffectParams?.ToModel(),
                IgnoreProtectionEffectsParams = type.IgnoreProtectionEffectsParams?.ToModel(),
                ChangeEffectTargetParams = type.ChangeEffectTargetParams?.ToModel(),
                MultiplierDependsOnRelation = type.MultiplierDependsOnRelation,
            };
        }

        public static SkillUpgrade ToModel(this SharedModel.Meta.Skills.SkillBonus bonus)
        {
            return new SkillUpgrade()
            {
                SkillBonusType = bonus.SkillBonusType.ToString(),
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
                Name = stage.Name.ToModel(),
                AreaId = (int)areaTypeId,
                RegionId = (int)regionTypeId,
                Difficulty = stage._difficulty.ToString(),
                StageId = stage.Id,
            };
        }
    }
}
