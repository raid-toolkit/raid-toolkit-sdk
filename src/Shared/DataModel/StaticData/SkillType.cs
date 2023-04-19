using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Raid.Toolkit.DataModel.Enums;

namespace Raid.Toolkit.DataModel
{
    public class Skill
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("level")]
        public int Level;
    }

    public abstract class BaseSkillData
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("cooldown")]
        public int Cooldown;

        [JsonProperty("visibility")]
        public Visibility Visibility;

        [JsonProperty("unblockable")]
        public bool? Unblockable;

        [JsonProperty("effects")]
        public EffectType[]? Effects;

        [JsonProperty("upgrades"), Obsolete("Use SkillBonuses instead")]
        public SkillUpgrade[]? Upgrades;

        [JsonProperty("skillBonuses")]
        public SkillBonus[]? SkillBonuses;

        [JsonProperty("doesDamage")]
        public bool DoesDamage => Effects?.Any(effect => effect.KindId == EffectKindId.Damage) ?? false;

        [JsonProperty("targets")]
        public SkillTargets? Targets;

        [JsonProperty("group")]
        public SkillGroup? Group;

        [JsonProperty("useInTeamAttack")]
        public bool? UseInTeamAttack;

        [JsonProperty("useInCounterAttack")]
        public bool? UseInCounterAttack;
        // TODO: there's a lot more data here we could extract
    }

    public class SkillType : BaseSkillData
    {
        [JsonProperty("name")]
        public LocalizedText Name;

        [JsonProperty("description")]
        public LocalizedText Description;
    }

    public class SkillSnapshot : BaseSkillData
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("level")]
        public int Level;

        public SkillSnapshot(SkillType skill)
        {
            Name = skill.Name.Localize();
            Description = skill.Description.Localize();
            TypeId = skill.TypeId;
            Cooldown = skill.Cooldown;
            Visibility = skill.Visibility;
            Unblockable = skill.Unblockable;
            Effects = skill.Effects;
            Upgrades = skill.Upgrades;
            SkillBonuses = skill.SkillBonuses;
            Targets = skill.Targets;
            Group = skill.Group;
            UseInCounterAttack = skill.UseInCounterAttack;
            UseInTeamAttack = skill.UseInTeamAttack;
        }
    }

    public enum EffectGroup
    {
        Active,
        Passive,
    }

    public enum EffectKindGroup
    {
        Undefined = 1,
        StatusDebuff = 2,
        ResistibleInstantDebuff = 3,
        EffectThatApplyStatusDebuffs = 4,
        EffectsThatBlockStatusDebuffs = 5,
        StatusBuff = 6,
        InstantBuff = 7,
        ControlEffects = 8,
    }

    public enum EffectKindId
    {
        Revive = 0,
        Heal = 1000,
        StartOfStatusBuff = 2000,
        BlockDamage = 2001,
        BlockDebuff = 2002,
        ContinuousHeal = 2003,
        Shield = 2004,
        StatusCounterattack = 2005,
        ReviveOnDeath = 2006,
        ShareDamage = 2007,
        Unkillable = 2008,
        DamageCounter = 2009,
        ReflectDamage = 2010,
        HitCounterShield = 2012,
        Invisible = 2013,
        ReduceDamageTaken = 2014,
        CrabShell = 2015,
        CritShield = 2016,
        Enrage = 2018,
        PoisonCloud = 2020,
        BoneShield = 2025,
        Cocoon = 2021,
        StoneSkin = 2022,
        NewbieDefence = 2024,
        LightOrbs = 2026,
        Taunt = 2027,
        StatusIncreaseAttack = 2101,
        StatusIncreaseDefence = 2102,
        StatusIncreaseSpeed = 2103,
        StatusIncreaseCriticalChance = 2104,
        StatusChangeDamageMultiplier = 2105,
        StatusIncreaseAccuracy = 2106,
        StatusIncreaseCriticalDamage = 2107,
        StatusIncreaseResistance = 2108,
        EndOfStatusBuff = 2999,
        StartOfStatusDebuff = 3000,
        Freeze = 3001,
        Provoke = 3002,
        Sleep = 3003,
        Stun = 3004,
        BlockHeal = 3005,
        BlockActiveSkills = 3006,
        ContinuousDamage = 3007,
        BlockBuffs = 3008,
        TimeBomb = 3009,
        IncreaseDamageTaken = 3010,
        BlockRevive = 3011,
        Mark = 3012,
        LifeDrainOnDamage = 3013,
        AoEContinuousDamage = 3014,
        Fear = 3015,
        IncreasePoisoning = 3016,
        BlockPassiveSkills = 3017,
        ElectricMark = 3018,
        Petrification = 3019,
        MirrorDamage = 3020,
        FireMark = 3021,
        MarkOfMadness = 3022,
        Polymorph = 3024,
        StatusReduceAttack = 3101,
        StatusReduceDefence = 3102,
        StatusReduceSpeed = 3103,
        StatusReduceCriticalChance = 3104,
        StatusReduceAccuracy = 3105,
        StatusReduceCriticalDamage = 3106,
        StatusReduceResistance = 3107,
        EndOfStatusDebuff = 3999,
        ApplyBuff = 4000,
        IncreaseStamina = 4001,
        RemoveDebuff = 4003,
        ActivateSkill = 4004,
        ShowSecretSkill = 4005,
        TeamAttack = 4006,
        ExtraTurn = 4007,
        LifeShare = 4008,
        ReduceCooldown = 4009,
        ReduceDebuffLifetime = 4010,
        IncreaseBuffLifetime = 4011,
        PassiveCounterattack = 4012,
        PassiveChangeStats = 4013,
        PassiveBlockDebuff = 4014,
        PassiveBonus = 4015,
        MultiplyBuff = 4016,
        PassiveReflectDamage = 4017,
        PassiveShareDamage = 4018,
        IncreaseShield = 4020,
        ReturnDebuffs = 4021,
        TransferDebuff = 4022,
        RestoreDestroyedHp = 4023,
        PassiveUnkillable = 4024,
        EndOfInstantBuff = 4999,
        ApplyDebuff = 5000,
        ReduceStamina = 5001,
        StealBuff = 5002,
        RemoveBuff = 5003,
        IncreaseCooldown = 5004,
        ReduceBuffLifetime = 5005,
        SwapHealth = 5007,
        IncreaseDebuffLifetime = 5008,
        DestroyHp = 5009,
        Detonate = 5010,
        MultiplyDebuff = 5011,
        ReduceShield = 5012,
        PassiveBlockBuff = 5013,
        DestroyStats = 5014,
        ApplyOrProlongDebuff = 5015,
        EndOfInstantDebuff = 5999,
        Damage = 6000,
        HitTypeModifier = 7000,
        ChangeDefenceModifier = 7001,
        ChangeEffectAccuracy = 7002,
        MultiplyEffectChance = 7003,
        ChangeDamageMultiplier = 7004,
        IgnoreProtectionEffects = 7005,
        ChangeCalculatedDamage = 7006,
        ChangeEffectRepeatCount = 7007,
        ChangeDestroyHpAmount = 7008,
        ChangeHealMultiplier = 7009,
        ChangeStaminaModifier = 7010,
        ChangeShieldMultiplier = 7011,
        ChangeEffectResistance = 7012,
        IgnoreDefenceModifier = 7013,
        ExcludeHitType = 7014,
        Summon = 8000,
        CopyHero = 8001,
        GrowHydraHead = 8002,
        ChangeEffectTarget = 9000,
        CancelEffect = 9001,
        ForceStatusEffectTick = 9002,
        ChangeSkyWrathCounter = 9005,
        UpdateCombo = 9006,
        SetVoidAbyssCounter = 9007,
        SetHydraHitCounter = 9008,
        SetHeroCounter = 9009,
        StatusBanish = 9010,
        Banish = 9011,
        ChangeSkillTarget = 9013,
        SetShieldHitCounter = 9012,
        SetLightOrbsStackCount = 9014,
        HungerCounter = 9020,
        SetHungerCounter = 9021,
        PlaceHungerCounter = 9022,
        Devour = 9023,
        Devoured = 9024,
        Digestion = 9025,
        SetSleepCounter = 9026,
        ApplyCounter = 9027,
        PassiveBlockEffect = 9030,
        Transformation = 9050,
        CancelTransformation = 9051,
        EffectDurationModifier = 10000,
        ChangeEffectProtection = 10001,
        CheckTargetForCondition = 11000,
        EffectContainer = 11001,
        ActionForVisualization = 11010,
        SleepCounter = 12000,
        SkyWrath = 12001,
        VoidAbyss = 12002,
        HydraHitCounter = 12003,

    }

    public enum SkillTargets
    {
        Producer = 0,
        AliveAllies = 1,
        AliveEnemies = 2,
        DeadAllies = 3,
        DeadEnemies = 4,
        AllAllies = 5,
        AllEnemies = 6,
        AliveAlliesExceptProducer = 7,
        AliveEnemiesIncludeInvisible = 8
    }

    public enum SkillGroup
    {
        Active = 0,
        Passive = 1
    }

    public enum EffectTargetType
    {
        Target = 0,
        Producer = 1,
        RelationTarget = 2,
        RelationProducer = 3,
        Owner = 4,
        RandomAlly = 5,
        RandomEnemy = 6,
        AllAllies = 7,
        AllEnemies = 8,
        AllDeadAllies = 9,
        RandomDeadAlly = 13,
        RandomDeadEnemy = 14,
        MostInjuredAlly = 19,
        MostInjuredEnemy = 20,
        Boss = 22,
        RandomRevivableAlly = 25,
        OwnerAllies = 26,
        AllHeroes = 29,
        ActiveHero = 31,
        AllyWithLowestMaxHp = 32,
        HeroCausedRelationUnapply = 33,
        HeroThatKilledProducer = 34,
        RelationTargetDuplicates = 35,
        AllyWithLowestStamina = 36,
        AllyWithHighestStamina = 37,
        EnemyWithLowestStamina = 38,
        EnemyWithHighestStamina = 39,
        RelationProducerOrTeamAttackInitiator = 40,
        RandomEnemyWithMaxBuffsCount = 41,
        RandomEnemyWithMarkAppliedByProducer = 42,
        RandomEnemyWithNotProtectedBuff = 43,
    }

    public enum TargetExclusion
    {
        Target,
        Producer,
        RelationTarget,
        RelationProducer,
    }

    public enum StatusEffectTypeId
    {
        Stun = 10,
        Freeze = 20,
        Sleep = 30,
        Provoke = 40,
        Counterattack = 50,
        BlockDamage = 60,
        BlockHeal100p = 70,
        BlockHeal50p = 71,
        ContinuousDamage5p = 80,
        ContinuousDamage025p = 81,
        ContinuousHeal075p = 90,
        ContinuousHeal15p = 91,
        BlockDebuff = 100,
        BlockBuffs = 110,
        IncreaseAttack25 = 120,
        IncreaseAttack50 = 121,
        DecreaseAttack25 = 130,
        DecreaseAttack50 = 131,
        IncreaseDefence30 = 140,
        IncreaseDefence60 = 141,
        DecreaseDefence30 = 150,
        DecreaseDefence60 = 151,
        IncreaseSpeed15 = 160,
        IncreaseSpeed30 = 161,
        DecreaseSpeed15 = 170,
        DecreaseSpeed30 = 171,
        IncreaseAccuracy25 = 220,
        IncreaseAccuracy50 = 221,
        DecreaseAccuracy25 = 230,
        DecreaseAccuracy50 = 231,
        IncreaseCriticalChance15 = 240,
        IncreaseCriticalChance30 = 241,
        DecreaseCriticalChance15 = 250,
        DecreaseCriticalChance30 = 251,
        IncreaseCriticalDamage15 = 260,
        IncreaseCriticalDamage30 = 261,
        DecreaseCriticalDamage15p = 270,
        DecreaseCriticalDamage25p = 271,
        Shield = 280,
        BlockActiveSkills = 290,
        ReviveOnDeath = 300,
        ShareDamage50 = 310,
        ShareDamage25 = 311,
        Unkillable = 320,
        TimeBomb = 330,
        DamageCounter = 340,
        IncreaseDamageTaken25 = 350,
        IncreaseDamageTaken15 = 351,
        BlockRevive = 360,
        ArtifactSet_Shield = 370,
        ReflectDamage15 = 410,
        ReflectDamage30 = 411,
        MinotaurIncreaseDamage = 420,
        MinotaurIncreaseDamageTaken = 430,
        HydraNeckIncreaseDamageTaken = 431,
        Mark = 440,
        HitCounterShield = 450,
        LifeDrainOnDamage10p = 460,
        Burn = 470,
        Invisible1 = 480,
        Invisible2 = 481,
        Fear1 = 490,
        Fear2 = 491,
        IncreasePoisoning25 = 500,
        IncreasePoisoning50 = 501,
        ReduceDamageTaken15 = 510,
        ReduceDamageTaken25 = 511,
        CrabShell = 520,
        CritShield25 = 530,
        CritShield50 = 531,
        CritShield75 = 532,
        CritShield100 = 533,
        SkyWrath = 540,
        Enrage = 550,
        BlockPassiveSkills = 560,
        StatusBanish = 570,
        VoidAbyss = 580,
        ElectricMark = 590,
        Cocoon = 600,
        PoisonCloud = 610,
        SimpleStoneSkin = 620,
        ReflectiveStoneSkin = 621,
        Petrification = 630,
        MirrorDamage = 640,
        BloodRage = 650,
        HydraHitCounter = 660,
        NewbieDefence = 670,
        HungerCounter = 680,
        Devoured = 690,
        Digestion = 700,
        IncreaseResistance25 = 710,
        IncreaseResistance50 = 711,
        DecreaseResistance25 = 720,
        DecreaseResistance50 = 721,
        BoneShield20 = 730,
        BoneShield30 = 731,
        FireMark = 740,
        MarkOfMadness = 750,
        LightOrbs = 760,
        Polymorph = 770,
        Taunt = 780,
        SleepCounter = 790,
    }

    public enum LifetimeUpdateType
    {
        OnStartTurn,
        OnEndTurn,
        Custom,
    }

    public enum ApplyMode
    {
        Unresistable,
        Guaranteed,
    }

    public enum ProtectionMode
    {
        ProtectedFromEnemies,
        FullyProtected,
    }

    public enum UnapplyEffectTarget
    {
        Target = 1,
        Producer = 2,
        RandomAllyExcludingProducer = 3,
    }

    public enum UnapplyEffectMode
    {
        Selected,
        AllExceptSelected,
    }

    public enum HitType
    {
        Normal,
        Crushing,
        Critical,
        Glancing,
    }

    public enum ElementRelation
    {
        Disadvantage = -1, // 0xFFFFFFFF
        Neutral = 0,
        Advantage = 1,
    }

    public enum EvenMode
    {
        Average,
        Highest,
        Lowest,
    }

    public enum ActivateSkillOwner
    {
        Producer,
        Target,
    }

    public enum SkillToChange
    {
        Random = 1,
        ByIndex = 2,
        SkillOfCurrentContext = 3,
        All = 4,
    }

    public enum AppliedEffectType
    {
        Buff,
        Debuff,
    }

    public enum CrabShellLayerType
    {
        First,
        Second,
        Third,
    }

    public enum PassiveBonus
    {
        Heal = 1,
        ShieldCreation = 2,
        StaminaRecovery = 3,
        ArtifactSetStats = 4,
        AuraEfficiency = 5,
    }

    public enum Visibility
    {
        Visible = 0,
        HiddenOnHud = 1,
        HiddenOnHudWithVisualization = 2,
        HiddenOnHudVisibleForAl = 3,
        HiddenOnHudWithVisualisationVisibleForAI = 4
    }

    public class TargetParams
    {
        [JsonProperty("targetType")]
        public EffectTargetType? TargetType;

        [JsonProperty("exclusion")]
        public TargetExclusion? Exclusion;

        [JsonProperty("exclusive")]
        public bool Exclusive;

        [JsonProperty("firstHitInSelected")]
        public bool FirstHitInSelected;

        [JsonProperty("condition")]
        public string? Condition;
    }

    public class EffectRelation
    {
        [JsonProperty("effectTypeId")]
        public int? EffectTypeId;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<EffectKindId>? EffectKindIds;

        [JsonProperty("effectKindGroups")]
        public IReadOnlyList<EffectKindGroup>? EffectKindGroups;

        [JsonProperty("phase")]
        public IReadOnlyList<BattlePhaseId>? Phases;

        [JsonProperty("activateOnGlancingHit")]
        public bool ActivateOnGlancingHit;
    }

    public class StatusEffectParams
    {
        [JsonProperty("strengthInFamily")]
        public int StrengthInFamily;

        [JsonProperty("forcedTickAllowed")]
        public bool ForcedTickAllowed;

        [JsonProperty("lifetimeUpdateType")]
        public LifetimeUpdateType LifetimeUpdateType;

        [JsonProperty("unapplyWhenProducerDied")]
        public bool? UnapplyWhenProducerDied;

        [JsonProperty("skipProcessingWhenJustApplied")]
        public bool? SkipProcessingWhenJustApplied;
    }

    public class StatusEffectInfo
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("duration")]
        public int Duration;

        [JsonProperty("ignoreEffectsLimit")]
        public bool? IgnoreEffectsLimit;

        [JsonProperty("applyMode")]
        public ApplyMode? ApplyMode;

        [JsonProperty("protection")]
        public Protection? Protection;

        [JsonProperty("durationFormula")]
        public string? DurationFormula;
    }

    public class Protection
    {
        [JsonProperty("mode")]
        public ProtectionMode Mode;

        [JsonProperty("chance")]
        public double? Chance;
    }

    public class ApplyStatusEffectParams
    {
        [JsonProperty("statusEffectInfos")]
        public IReadOnlyList<StatusEffectInfo>? StatusEffectInfos;
    }

    public class UnapplyStatusEffectParams
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("statusEffectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId>? StatusEffectTypeIds;

        [JsonProperty("effectKindGroups")]
        public IReadOnlyList<StatusEffectTypeId>? EffectKindGroups;

        [JsonProperty("unapplyMode")]
        public UnapplyEffectMode UnapplyMode;

        [JsonProperty("removeFrom")]
        public UnapplyEffectTarget? RemoveFrom;

        [JsonProperty("applyTo")]
        public UnapplyEffectTarget? ApplyTo;

    }

    public class TransferDebuffParams
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("statusEffectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId>? StatusEffectTypeIds;

        [JsonProperty("unapplyMode")]
        public UnapplyEffectMode UnapplyMode;

        [JsonProperty("includeProducer")]
        public bool IncludeProducer;

        [JsonProperty("applyMode")]
        public ApplyMode? ApplyMode;

        [JsonProperty("removeFrom")]
        public EffectTargetType RemoveFrom;

        [JsonProperty("applyTo")]
        public EffectTargetType ApplyTo;
    }

    public class DamageParams
    {
        [JsonProperty("hitType")]
        public HitType? HitType;

        [JsonProperty("elementRelation")]
        public ElementRelation? ElementRelation;

        [JsonProperty("defenceModifier")]
        public double DefenceModifier;

        [JsonProperty("isFixed")]
        public bool IsFixed;

        [JsonProperty("doesNotCountAsHit")]
        public bool DoesNotCountAsHit;

        [JsonProperty("increaseCriticalHitChance")]
        public double? IncreaseCriticalHitChance;

        [JsonProperty("ignoreStatusEffectReduction")]
        public bool? IgnoreStatusEffectReduction;

        [JsonProperty("valueCapExpression")]
        public string? ValueCapExpression;

        [JsonProperty("specificDamageType")]
        public SpecificDamageType? SpecificDamageType;
    }

    public enum SpecificDamageType
    {
        Skeletons = 0,
        LightOrbs = 1
    }

    public class HealParams
    {
        [JsonProperty("canBeCritical")]
        public bool CanBeCritical;
    }

    public class EvenParams
    {
        [JsonProperty("evenMode")]
        public EvenMode EvenMode;
    }

    public class ChangeStatParams
    {
        [JsonProperty("statKindId")]
        public StatKindId Param;
    }

    public class ActivateSkillParams
    {
        [JsonProperty("skillIndex")]
        public int SkillIndex;

        [JsonProperty("skillOwner")]
        public ActivateSkillOwner SkillOwner;

        [JsonProperty("targetExpression")]
        public string TargetExpression;
    }

    public class ShowHiddenSkillParams
    {
        [JsonProperty("skillTypeId")]
        public int SkillTypeId;

        [JsonProperty("shouldHide")]
        public bool ShouldHide;
    }

    public class ChangeSkillCooldownParams
    {
        [JsonProperty("turns")]
        public int Turns;

        [JsonProperty("skillIndex")]
        public int? SkillIndex;

        [JsonProperty("isRandomSkill")]
        public bool? IsRandomSkill;

        [JsonProperty("skillToChange")]
        public SkillToChange SkillToChange;
    }

    public class ChangeEffectLifetimeParams
    {
        [JsonProperty("type")]
        public AppliedEffectType Type;

        [JsonProperty("turns")]
        public int Turns;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId>? EffectTypeIds;
    }

    public class ShareDamageParams
    {
        [JsonProperty("targetDamageCutPerc")]
        public double TargetDamageCutPerc;

        [JsonProperty("transferedDamagePerc")]
        public double TransferedDamagePerc;

        [JsonProperty("defenceModifier")]
        public double? DefenceModifier;
    }

    public class BlockEffectParams
    {
        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<int>? EffectTypeIds;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<int>? EffectKindIds;

        [JsonProperty("blockGuaranteed")]
        public bool? BlockGuaranteed;

        [JsonProperty("blockAllExcludeSelected")]
        public bool? BlockAllExcludeSelected { get; set; }
    }

    public class TeamAttackParams
    {
        public int TeammatesCount;

        public bool ExcludeProducerFromAttack;

        public IReadOnlyList<int>? PreferredHeroTypes;

        public bool? AlwaysUsePreferredWhenPossible;

        public string? AllySelectorExpression;
    }

    public class SummonParams
    {
        [JsonProperty("baseTypeId")]
        public int BaseTypeId;

        [JsonProperty("ascendLevelFormula")]
        public string AscendLevelFormula;

        [JsonProperty("gradeFormula")]
        public string GradeFormula;

        [JsonProperty("levelFormula")]
        public string LevelFormula;

        [JsonProperty("removeAfterDeath")]
        public bool RemoveAfterDeath;

        [JsonProperty("slotsLimit")]
        public int SlotsLimit;
    }

    public class DestroyHpParams
    {
        [JsonProperty("ignoreShield")]
        public bool IgnoreShield;
    }

    public class ReviveParams
    {
        [JsonProperty("healPercent")]
        public double HealPercent;

        [JsonProperty("ignoreBlockRevive")]
        public bool IgnoreBlockRevive;
    }

    public class CounterattackParams
    {
        [JsonProperty("skillIndex")]
        public int SkillIndex;

        [JsonProperty("noPenalty")]
        public bool NoPenalty;
    }

    public class ForceStatusEffectTickParams
    {
        [JsonProperty("ticks")]
        public int Ticks;

        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId>? EffectTypeIds;

        [JsonProperty("effectCount")]
        public int EffectCount;
    }

    public class CrabShellLayer
    {
        [JsonProperty("type")]
        public CrabShellLayerType Type;

        [JsonProperty("multiplierFormula")]
        public string? MultiplierFormula;

        [JsonProperty("conditionFormula")]
        public string? ConditionFormula;
    }

    public class CrabShellParams
    {
        [JsonProperty("layers")]
        public IReadOnlyList<CrabShellLayer>? Layers;
    }

    public class ReturnDebuffsParams
    {
        [JsonProperty("applyMode")]
        public ApplyMode? ApplyMode;
    }

    public class HitTypeParams
    {
        [JsonProperty("hitTypeToChange")]
        public HitType? HitTypeToChange;

        [JsonProperty("hitType")]
        public HitType HitType;
    }

    public class PassiveBonusParams
    {
        [JsonProperty("bonus")]
        public PassiveBonus Bonus;
    }

    public class MultiplyStatusEffectParams
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("turnsModifier")]
        public int TurnsModifier;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<EffectKindId>? EffectKindIds;

        [JsonProperty("targetSelectorExpression")]
        public string? TargetSelectorExpression;

        [JsonProperty("turnsChangeMode")]
        public TurnsChangeMode? TurnsChangeMode;
    }

    public enum TurnsChangeMode
    {
        Modify = 0,
        Replace = 1
    }

    public class IgnoreProtectionEffectsParams
    {
        [JsonProperty("ignoreBlockDamage")]
        public bool IgnoreBlockDamage;

        [JsonProperty("ignoreShield")]
        public bool IgnoreShield;

        [JsonProperty("ignoreUnkillable")]
        public bool IgnoreUnkillable;
    }

    public class ChangeEffectTargetParams
    {
        [JsonProperty("overrideApplyMode")]
        public bool OverrideApplyMode;

        [JsonProperty("applyMode")]
        public ApplyMode? ApplyMode;
    }

    public class PlaceHungerCounterParams
    {
        [JsonProperty("iterationsBetweenDevouring")]
        public int IterationsBetweenDevouring;
    }

    public class NewbieDefenceParams
    {
        [JsonProperty("changeDamageFactor")]
        public double ChangeDamageFactor;
    }

    public class CocoonParams
    {
        [JsonProperty("stunTurns")]
        public int StunTurns;
    }

    public class PetrificationParams
    {
        [JsonProperty("generalChangeDamageFactor")]
        public double GeneralChangeDamageFactor;

        [JsonProperty("timeBombChangeDamageFactor")]
        public double TimeBombChangeDamageFactor;
    }

    public class StoneSkinParams
    {
        [JsonProperty("reflectChance")]
        public double? ReflectChance;

        [JsonProperty("timeBombChangeDamageFactor")]
        public double TimeBombChangeDamageFactor;

        [JsonProperty("generalChangeDamageFactor")]
        public double GeneralChangeDamageFactor;

        [JsonProperty("petrificationTurns")]
        public int? PetrificationTurns;
    }

    public class DestroyStatsParams
    {
        [JsonProperty("statKindId")]
        public StatKindId StatKindId;

        [JsonProperty("maxDestructionPercentFormula")]
        public string? MaxDestructionPercentFormula;
    }

    public class GrowHydraHeadParams
    {
        [JsonProperty("growSelfProbability")]
        public double GrowSelfProbability;
    }

    public class DevourParams
    {
        [JsonProperty("digestionLifetimeFormula")]
        public string DigestionLifetimeFormula;

        [JsonProperty("digestionStrengthFormula")]
        public string DigestionStrengthFormula;
    }

    public class TransformationParams
    {
        [JsonProperty("variantId")]
        public int VariantId;
    }

    public class CancelTransformationParams
    {
        [JsonProperty("stamina")]
        public double Stamina;
        [JsonProperty("healthPercent")]
        public double HealthPercent;
    }

    public class EffectContainerParams
    {
        [JsonProperty("effect")]
        public EffectType? Effect;
    }

    public class ExcludeHitTypeParams
    {
        [JsonProperty("excludeGlancingHit")]
        public bool ExcludeGlancingHit;

        [JsonProperty("excludeCriticalHit")]
        public bool ExcludeCriticalHit;

        [JsonProperty("excludeCrushingHit")]
        public bool ExcludeCrushingHit;
    }

    public class ChangeShieldParams
    {
        [JsonProperty("shieldTypes")]
        public StatusEffectTypeId[]? ShieldTypes;
    }

    public class ChangeProtectionParams
    {
        [JsonProperty("protection")]
        public Protection? Protection;

        [JsonProperty("canReplaceStronger")]
        public bool CanReplaceStronger;
    }

    public class EffectType
    {
        [JsonProperty("id")]
        public int TypeId;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("multiplier"), Obsolete("Use multiplierFormula")]
        public string? Multiplier => MultiplierFormula;

        [JsonProperty("stack")]
        public int StackCount;

        [JsonProperty("kindId")]
        public EffectKindId KindId;

        // TODO: there's a lot more data here we could extract

        [JsonProperty("group")]
        public EffectGroup Group;

        [JsonProperty("targetParams")]
        public TargetParams? TargetParams;

        [JsonProperty("isEffectDescription")]
        public bool IsEffectDescription;

        [JsonProperty("considersDead")]
        public bool ConsidersDead;

        [JsonProperty("leaveThroughDeath")]
        public bool LeaveThroughDeath;

        [JsonProperty("doesntSetSkillOnCooldown")]
        public bool DoesntSetSkillOnCooldown;

        [JsonProperty("ignoresCooldown")]
        public bool IgnoresCooldown;

        [JsonProperty("isUnique")]
        public bool IsUnique;

        [JsonProperty("iterationChanceRolling")]
        public bool IterationChanceRolling;

        [JsonProperty("relation")]
        public EffectRelation? Relation;

        [JsonProperty("condition")]
        public string? Condition;

        [JsonProperty("chance")]
        public double? Chance;

        [JsonProperty("repeatChance")]
        public double? RepeatChance;

        [JsonProperty("statusEffectParams")]
        public StatusEffectParams? StatusParams;

        [JsonProperty("valueCap")]
        public string? ValueCap;

        [JsonProperty("applyInstantEffectMode")]
        public ApplyMode? ApplyInstantEffectMode;

        [JsonProperty("persistsThroughRounds")]
        public bool PersistsThroughRounds;

        [JsonProperty("snapshotRequired")]
        public bool SnapshotRequired;

        [JsonProperty("ignoredEffects")]
        public IReadOnlyList<EffectKindId>? IgnoredEffects;

        [JsonProperty("applyStatusEffectParams")]
        public ApplyStatusEffectParams? ApplyStatusEffectParams;

        [JsonProperty("unapplyStatusEffectParams")]
        public UnapplyStatusEffectParams? UnapplyStatusEffectParams;

        [JsonProperty("transferDebuffParams")]
        public TransferDebuffParams? TransferDebuffParams;

        [JsonProperty("damageParams")]
        public DamageParams? DamageParams;

        [JsonProperty("healParams")]
        public HealParams? HealParams;

        [JsonProperty("evenParams")]
        public EvenParams? EvenParams;

        [JsonProperty("changeStatParams")]
        public ChangeStatParams? ChangeStatParams;

        [JsonProperty("activateSkillParams")]
        public ActivateSkillParams? ActivateSkillParams;

        [JsonProperty("showHiddenSkillParams")]
        public ShowHiddenSkillParams? ShowHiddenSkillParams;

        [JsonProperty("changeSkillCooldownParams")]
        public ChangeSkillCooldownParams? ChangeSkillCooldownParams;

        [JsonProperty("changeEffectLifetimeParams")]
        public ChangeEffectLifetimeParams? ChangeEffectLifetimeParams;

        [JsonProperty("shareDamageParams")]
        public ShareDamageParams? ShareDamageParams;

        [JsonProperty("blockEffectParams")]
        public BlockEffectParams? BlockEffectParams;

        [JsonProperty("summonParams")]
        public SummonParams? SummonParams;

        [JsonProperty("teamAttackParams")]
        public TeamAttackParams? TeamAttackParams;

        [JsonProperty("destroyHpParams")]
        public DestroyHpParams? DestroyHpParams;

        [JsonProperty("reviveParams")]
        public ReviveParams? ReviveParams;

        [JsonProperty("counterattackParams")]
        public CounterattackParams? CounterattackParams;

        [JsonProperty("forceTickParams")]
        public ForceStatusEffectTickParams? ForceTickParams;

        [JsonProperty("crabShellParams")]
        public CrabShellParams? CrabShellParams;

        [JsonProperty("returnDebuffsParams")]
        public ReturnDebuffsParams? ReturnDebuffsParams;

        [JsonProperty("hitTypeParams")]
        public HitTypeParams? HitTypeParams;

        [JsonProperty("passiveBonusParams")]
        public PassiveBonusParams? PassiveBonusParams;

        [JsonProperty("multiplyStatusEffectParams")]
        public MultiplyStatusEffectParams? MultiplyStatusEffectParams;

        [JsonProperty("ignoreProtectionEffectsParams")]
        public IgnoreProtectionEffectsParams? IgnoreProtectionEffectsParams;

        [JsonProperty("changeEffectTargetParams")]
        public ChangeEffectTargetParams? ChangeEffectTargetParams;

        [JsonProperty("multiplierFormula")]
        public string? MultiplierFormula;

        [JsonProperty("multiplierNotEvaluatedByAI")]
        public bool MultiplierNotEvaluatedByAI;

        [JsonProperty("isContainer")]
        public bool IsContainer;

        [JsonProperty("placeHungerCounterParams")]
        public PlaceHungerCounterParams? PlaceHungerCounterParams;

        [JsonProperty("newbieDefenceParams")]
        public NewbieDefenceParams? NewbieDefenceParams;

        [JsonProperty("cocoonParams")]
        public CocoonParams? CocoonParams;

        [JsonProperty("petrificationParams")]
        public PetrificationParams? PetrificationParams;

        [JsonProperty("stoneSkinParams")]
        public StoneSkinParams? StoneSkinParams;

        [JsonProperty("destroyStatsParams")]
        public DestroyStatsParams? DestroyStatsParams;

        [JsonProperty("growHydraHeadParams")]
        public GrowHydraHeadParams? GrowHydraHeadParams;

        [JsonProperty("devourParams")]
        public DevourParams? DevourParams;

        [JsonProperty("transformationParams")]
        public TransformationParams? TransformationParams;

        [JsonProperty("cancelTransformationParams")]
        public CancelTransformationParams? CancelTransformationParams;

        [JsonProperty("effectContainerParams")]
        public EffectContainerParams? EffectContainerParams;

        [JsonProperty("excludeHitTypeParams")]
        public ExcludeHitTypeParams? ExcludeHitTypeParams;

        [JsonProperty("changeShieldParams")]
        public ChangeShieldParams? ChangeShieldParams;

        [JsonProperty("changeProtectionParams")]
        public ChangeProtectionParams? ChangeProtectionParams;
    }

    [Obsolete("Use SkillBonus")]
    public class SkillUpgrade
    {
        [JsonProperty("type")]
        public string? SkillBonusType;

        [JsonProperty("value")]
        public double Value;
    }

    public class SkillBonus
    {
        [JsonProperty("type")]
        public SkillBonusType SkillBonusType;

        [JsonProperty("value")]
        public double Value;
    }

    public enum SkillBonusType
    {
        Attack = 0,
        Health = 1,
        EffectChance = 2,
        CooltimeTurn = 3,
        ShieldCreation = 4
    }

    public enum BattlePhaseId
    {
        Unknown = 0,
        BattleStarted = 10,
        BattleFinished = 11,
        BeforeTurnStarted = 20,
        AfterTurnStarted = 21,
        TurnFinished = 22,
        ImmunitiesProcessing = 32,
        RoundStarted = 30,
        RoundFinished = 31,
        BeforeEffectProcessed = 40,
        BeforeEffectProcessedOnTarget = 41,
        BeforeEffectAppliedOnTarget = 42,
        AfterEffectAppliedOnTarget = 43,
        AfterEffectProcessedOnTarget = 44,
        AfterEffectProcessed = 45,
        BeforeStatusEffectAppliedOnTarget = 250,
        EffectBlocked = 190,
        EffectExpired = 46,
        BeforeEffectChanceRolling = 47,
        AfterEffectChanceRolling = 48,
        TargetContextHasJustBeenCreated = 49,
        AfterHitTypeCalculated = 57,
        BeforeDamageCalculated = 50,
        AfterDamageCalculated = 51,
        StoneSkinAbsorptionProcessing = 200,
        BeforeDamageDealt = 52,
        AfterHealthReduced = 56,
        UnkillableProcessing = 58,
        AfterDamageDealt = 53,
        BlockDamageProcessing = 54,
        AfterDamageContextCreated = 55,
        CocoonProcessing = 59,
        IgnoreDefenceModifierProcessing = 240,
        BeforeHealCalculated = 62,
        BeforeHealDealt = 60,
        AfterHealDealt = 61,
        AllHeroesDeathProcessed = 70,
        HeroDead = 71,
        AfterSkillEffectsProcessed = 72,
        AfterHeroSummoned = 80,
        BeforeAppliedEffectsUpdate = 100,
        FearProcessing = 111,
        BeforeSkillProcessed = 114,
        AfterSkillUsed = 112,
        AfterAllSkillsUsed = 113,
        AfterStatusEffectToApplySelected = 120,
        CancelEffectProcessing = 130,
        BeforeEffectUnappliedFromHero = 140,
        AfterEffectUnappliedFromHero = 141,
        BeforeDestroyHpDealt = 150,
        CrabShellProcessing = 160,
        BeforeStaminaChanged = 170,
        StatusReviveOnDeathProcessing = 180,
        PassiveReviveOnDeathProcessing = 181,
        AfterHeroDevoured = 210,
        DigestionAbsorptionProcessing = 220,
        HydraHeadGrown = 230,
    }
}
