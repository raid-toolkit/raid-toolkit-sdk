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
        public bool Unblockable;

        [JsonProperty("effects")]
        public EffectType[] Effects;

        [JsonProperty("upgrades")]
        public SkillUpgrade[] Upgrades;

        [JsonProperty("doesDamage")]
        public bool DoesDamage => Effects?.Any(effect => effect.KindId == EffectKindId.Damage) ?? false;
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
        }
    }

    public enum EffectGroup
    {
        Active,
        Passive,
    }

    public enum EffectKindId
    {
        Revive = 0,
        Heal = 1000, // 0x000003E8
        StartOfStatusBuff = 2000, // 0x000007D0
        BlockDamage = 2001, // 0x000007D1
        BlockDebuff = 2002, // 0x000007D2
        ContinuousHeal = 2003, // 0x000007D3
        Shield = 2004, // 0x000007D4
        StatusCounterattack = 2005, // 0x000007D5
        ReviveOnDeath = 2006, // 0x000007D6
        ShareDamage = 2007, // 0x000007D7
        Unkillable = 2008, // 0x000007D8
        DamageCounter = 2009, // 0x000007D9
        ReflectDamage = 2010, // 0x000007DA
        HitCounterShield = 2012, // 0x000007DC
        Invisible = 2013, // 0x000007DD
        ReduceDamageTaken = 2014, // 0x000007DE
        CrabShell = 2015, // 0x000007DF
        CritShield = 2016, // 0x000007E0
        SkyWrath = 2017, // 0x000007E1
        Enrage = 2018, // 0x000007E2
        VoidAbyss = 2019, // 0x000007E3
        StatusIncreaseAttack = 2101, // 0x00000835
        StatusIncreaseDefence = 2102, // 0x00000836
        StatusIncreaseSpeed = 2103, // 0x00000837
        StatusIncreaseCriticalChance = 2104, // 0x00000838
        StatusChangeDamageMultiplier = 2105, // 0x00000839
        StatusIncreaseAccuracy = 2106, // 0x0000083A
        StatusIncreaseCriticalDamage = 2107, // 0x0000083B
        EndOfStatusBuff = 2999, // 0x00000BB7
        StartOfStatusDebuff = 3000, // 0x00000BB8
        Freeze = 3001, // 0x00000BB9
        Provoke = 3002, // 0x00000BBA
        Sleep = 3003, // 0x00000BBB
        Stun = 3004, // 0x00000BBC
        BlockHeal = 3005, // 0x00000BBD
        BlockActiveSkills = 3006, // 0x00000BBE
        ContinuousDamage = 3007, // 0x00000BBF
        BlockBuffs = 3008, // 0x00000BC0
        TimeBomb = 3009, // 0x00000BC1
        IncreaseDamageTaken = 3010, // 0x00000BC2
        BlockRevive = 3011, // 0x00000BC3
        Mark = 3012, // 0x00000BC4
        LifeDrainOnDamage = 3013, // 0x00000BC5
        AoEContinuousDamage = 3014, // 0x00000BC6
        Fear = 3015, // 0x00000BC7
        IncreasePoisoning = 3016, // 0x00000BC8
        BlockPassiveSkills = 3017, // 0x00000BC9
        StatusReduceAttack = 3101, // 0x00000C1D
        StatusReduceDefence = 3102, // 0x00000C1E
        StatusReduceSpeed = 3103, // 0x00000C1F
        StatusReduceCriticalChance = 3104, // 0x00000C20
        StatusReduceAccuracy = 3105, // 0x00000C21
        StatusReduceCriticalDamage = 3106, // 0x00000C22
        EndOfStatusDebuff = 3999, // 0x00000F9F
        ApplyBuff = 4000, // 0x00000FA0
        IncreaseStamina = 4001, // 0x00000FA1
        RemoveDebuff = 4003, // 0x00000FA3
        ActivateSkill = 4004, // 0x00000FA4
        ShowHiddenSkill = 4005, // 0x00000FA5
        TeamAttack = 4006, // 0x00000FA6
        ExtraTurn = 4007, // 0x00000FA7
        LifeShare = 4008, // 0x00000FA8
        ReduceCooldown = 4009, // 0x00000FA9
        ReduceDebuffLifetime = 4010, // 0x00000FAA
        IncreaseBuffLifetime = 4011, // 0x00000FAB
        PassiveCounterattack = 4012, // 0x00000FAC
        PassiveChangeStats = 4013, // 0x00000FAD
        PassiveBlockDebuff = 4014, // 0x00000FAE
        PassiveBonus = 4015, // 0x00000FAF
        MultiplyBuff = 4016, // 0x00000FB0
        PassiveReflectDamage = 4017, // 0x00000FB1
        PassiveShareDamage = 4018, // 0x00000FB2
        IncreaseShield = 4020, // 0x00000FB4
        ReturnDebuffs = 4021, // 0x00000FB5
        TransferDebuff = 4022, // 0x00000FB6
        EndOfInstantBuff = 4999, // 0x00001387
        ApplyDebuff = 5000, // 0x00001388
        ReduceStamina = 5001, // 0x00001389
        StealBuff = 5002, // 0x0000138A
        RemoveBuff = 5003, // 0x0000138B
        IncreaseCooldown = 5004, // 0x0000138C
        ReduceBuffLifetime = 5005, // 0x0000138D
        PassiveDebuffLifetime = 5006, // 0x0000138E
        SwapHealth = 5007, // 0x0000138F
        IncreaseDebuffLifetime = 5008, // 0x00001390
        DestroyHp = 5009, // 0x00001391
        Detonate = 5010, // 0x00001392
        MultiplyDebuff = 5011, // 0x00001393
        ReduceShield = 5012, // 0x00001394
        EndOfInstantDebuff = 5999, // 0x0000176F
        Damage = 6000, // 0x00001770
        HitTypeModifier = 7000, // 0x00001B58
        ChangeDefenceModifier = 7001, // 0x00001B59
        ChangeEffectAccuracy = 7002, // 0x00001B5A
        MultiplyEffectChance = 7003, // 0x00001B5B
        ChangeDamageMultiplier = 7004, // 0x00001B5C
        IgnoreProtectionEffects = 7005, // 0x00001B5D
        ChangeCalculatedDamage = 7006, // 0x00001B5E
        ChangeEffectRepeatCount = 7007, // 0x00001B5F
        ChangeDestroyHpAmount = 7008, // 0x00001B60
        ChangeHealMultiplier = 7009, // 0x00001B61
        ChangeStaminaModifier = 7010, // 0x00001B62
        Summon = 8000, // 0x00001F40
        CopyHero = 8001, // 0x00001F41
        ChangeEffectTarget = 9000, // 0x00002328
        CancelEffect = 9001, // 0x00002329
        ForceStatusEffectTick = 9002, // 0x0000232A
        ChangeSkyWrathCounter = 9005, // 0x0000232D
        UpdateCombo = 9006, // 0x0000232E
        SetVoidAbyssCounter = 9007, // 0x0000232F
        StatusBanish = 9010, // 0x00002332
        Banish = 9011, // 0x00002333
        EffectDurationModifier = 10000, // 0x00002710
        CheckTargetForCondition = 11000, // 0x00002AF8
        EffectContainer = 11001, // 0x00002AF9
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
        RandomDeadAlly = 13, // 0x0000000D
        RandomDeadEnemy = 14, // 0x0000000E
        MostInjuredAlly = 19, // 0x00000013
        MostInjuredEnemy = 20, // 0x00000014
        Boss = 22, // 0x00000016
        RandomRevivableAlly = 25, // 0x00000019
        OwnerAllies = 26, // 0x0000001A
        AllHeroes = 29, // 0x0000001D
        ActiveHero = 31, // 0x0000001F
        AllyWithLowestMaxHp = 32, // 0x00000020
        HeroCausedRelationUnapply = 33, // 0x00000021
        HeroThatKilledProducer = 34, // 0x00000022
        RelationTargetDuplicates = 35, // 0x00000023
        AllyWithLowestStamina = 36, // 0x00000024
        AllyWithHighestStamina = 37, // 0x00000025
        EnemyWithLowestStamina = 38, // 0x00000026
        EnemyWithHighestStamina = 39, // 0x00000027
        RelationProducerOrTeamAttackInitiator = 40, // 0x00000028
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
        Stun = 10, // 0x0000000A
        Freeze = 20, // 0x00000014
        Sleep = 30, // 0x0000001E
        Provoke = 40, // 0x00000028
        Counterattack = 50, // 0x00000032
        BlockDamage = 60, // 0x0000003C
        BlockHeal100p = 70, // 0x00000046
        BlockHeal50p = 71, // 0x00000047
        ContinuousDamage5p = 80, // 0x00000050
        ContinuousDamage025p = 81, // 0x00000051
        ContinuousHeal075p = 90, // 0x0000005A
        ContinuousHeal15p = 91, // 0x0000005B
        BlockDebuff = 100, // 0x00000064
        BlockBuffs = 110, // 0x0000006E
        IncreaseAttack25 = 120, // 0x00000078
        IncreaseAttack50 = 121, // 0x00000079
        DecreaseAttack25 = 130, // 0x00000082
        DecreaseAttack50 = 131, // 0x00000083
        IncreaseDefence30 = 140, // 0x0000008C
        IncreaseDefence60 = 141, // 0x0000008D
        DecreaseDefence30 = 150, // 0x00000096
        DecreaseDefence60 = 151, // 0x00000097
        IncreaseSpeed15 = 160, // 0x000000A0
        IncreaseSpeed30 = 161, // 0x000000A1
        DecreaseSpeed15 = 170, // 0x000000AA
        DecreaseSpeed30 = 171, // 0x000000AB
        IncreaseAccuracy25 = 220, // 0x000000DC
        IncreaseAccuracy50 = 221, // 0x000000DD
        DecreaseAccuracy25 = 230, // 0x000000E6
        DecreaseAccuracy50 = 231, // 0x000000E7
        IncreaseCriticalChance15 = 240, // 0x000000F0
        IncreaseCriticalChance30 = 241, // 0x000000F1
        DecreaseCriticalChance15 = 250, // 0x000000FA
        DecreaseCriticalChance30 = 251, // 0x000000FB
        IncreaseCriticalDamage15 = 260, // 0x00000104
        IncreaseCriticalDamage30 = 261, // 0x00000105
        DecreaseCriticalDamage15p = 270, // 0x0000010E
        DecreaseCriticalDamage25p = 271, // 0x0000010F
        Shield = 280, // 0x00000118
        BlockActiveSkills = 290, // 0x00000122
        ReviveOnDeath = 300, // 0x0000012C
        ShareDamage50 = 310, // 0x00000136
        ShareDamage25 = 311, // 0x00000137
        Unkillable = 320, // 0x00000140
        TimeBomb = 330, // 0x0000014A
        DamageCounter = 340, // 0x00000154
        IncreaseDamageTaken25 = 350, // 0x0000015E
        IncreaseDamageTaken15 = 351, // 0x0000015F
        BlockRevive = 360, // 0x00000168
        ArtifactSetShield = 370, // 0x00000172
        ReflectDamage15 = 410, // 0x0000019A
        ReflectDamage30 = 411, // 0x0000019B
        MinotaurIncreaseDamage = 420, // 0x000001A4
        MinotaurIncreaseDamageTaken = 430, // 0x000001AE
        Mark = 440, // 0x000001B8
        HitCounterShield = 450, // 0x000001C2
        LifeDrainOnDamage10p = 460, // 0x000001CC
        Burn = 470, // 0x000001D6
        Invisible1 = 480, // 0x000001E0
        Invisible2 = 481, // 0x000001E1
        Fear1 = 490, // 0x000001EA
        Fear2 = 491, // 0x000001EB
        IncreasePoisoning25 = 500, // 0x000001F4
        IncreasePoisoning50 = 501, // 0x000001F5
        ReduceDamageTaken15 = 510, // 0x000001FE
        ReduceDamageTaken25 = 511, // 0x000001FF
        CrabShell = 520, // 0x00000208
        CritShield25 = 530, // 0x00000212
        CritShield50 = 531, // 0x00000213
        CritShield75 = 532, // 0x00000214
        CritShield100 = 533, // 0x00000215
        SkyWrath = 540, // 0x0000021C
        Enrage = 550, // 0x00000226
        BlockPassiveSkills = 560, // 0x00000230
        StatusBanish = 570, // 0x0000023A
        VoidAbyss = 580, // 0x00000244
    }

    public enum EffectKindGroup
    {
        Undefined = 1,
        StatusDebuff = 2,
        ResistibleInstantDebuff = 3,
        EffectThatApplyStatusDebuffs = 4,
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
    }

    public enum Visibility
    {
        Visible = 0,
        HiddenOnHud = 1,
        HiddenOnHudWithVisualization = 2,
        HiddenOnHudVisibleForAl = 3,
        HiddenOnHudWithVisualisationVisibleForAI = 4
    }

    public class EffectGroupStub
    { }

    public class TargetParamsStub
    {
        [JsonProperty("targetType")]
        public EffectTargetType TargetType;

        [JsonProperty("exclusion")]
        public TargetExclusion Exclusion;

        [JsonProperty("exclusive")]
        public bool Exclusive;

        [JsonProperty("firstHitInSelected")]
        public bool FirstHitInSelected;

        [JsonProperty("condition")]
        public string Condition;
    }

    public class EffectRelationStub
    {
        [JsonProperty("effectTypeId")]
        public int EffectTypeId;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<EffectKindId> EffectKindIds;

        [JsonProperty("effectKindGroups")]
        public IReadOnlyList<EffectKindGroup> EffectKindGroups;

        [JsonProperty("phase")]
        public IReadOnlyList<BattlePhaseId> Phases;

        [JsonProperty("activateOnGlancingHit")]
        public bool ActivateOnGlancingHit;
    }

    public class StatusEffectParamsStub
    {
        [JsonProperty("strengthInFamily")]
        public int StrengthInFamily;

        [JsonProperty("forcedTickAllowed")]
        public bool ForcedTickAllowed;

        [JsonProperty("lifetimeUpdateType")]
        public LifetimeUpdateType LifetimeUpdateType;

        [JsonProperty("unapplyWhenProducerDied")]
        public bool UnapplyWhenProducerDied;
    }

    public class StatusEffectInfoStub
    {
        [JsonProperty("typeId")]
        public int TypeId;

        [JsonProperty("duration")]
        public int Duration;

        [JsonProperty("ignoreEffectsLimit")]
        public bool IgnoreEffectsLimit;

        [JsonProperty("applyMode")]
        public ApplyMode ApplyMode;

        [JsonProperty("protection")]
        public Protection Protection;
    }

    public class Protection
    {
        [JsonProperty("mode")]
        public ProtectionMode Mode;

        [JsonProperty("chance")]
        public double? Chance;
    }

    public class ApplyStatusEffectParamsStub
    {
        [JsonProperty("statusEffectInfos")]
        public IReadOnlyList<StatusEffectInfoStub> StatusEffectInfos;
    }

    public class UnapplyStatusEffectParamsStub
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("statusEffectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId> StatusEffectTypeIds;

        [JsonProperty("unapplyMode")]
        public UnapplyEffectMode UnapplyMode;

        [JsonProperty("removeFrom")]
        public UnapplyEffectTarget RemoveFrom;

        [JsonProperty("applyTo")]
        public UnapplyEffectTarget ApplyTo;
    }

    public class TransferDebuffParamsStub
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("statusEffectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId> StatusEffectTypeIds;

        [JsonProperty("unapplyMode")]
        public UnapplyEffectMode UnapplyMode;

        [JsonProperty("includeProducer")]
        public bool IncludeProducer;

        [JsonProperty("applyMode")]
        public ApplyMode ApplyMode;

        [JsonProperty("removeFrom")]
        public EffectTargetType RemoveFrom;

        [JsonProperty("applyTo")]
        public EffectTargetType ApplyTo;
    }

    public class DamageParamsStub
    {
        [JsonProperty("hitType")]
        public HitType HitType;

        [JsonProperty("elementRelation")]
        public ElementRelation ElementRelation;

        [JsonProperty("defenceModifier")]
        public double DefenceModifier;

        [JsonProperty("isFixed")]
        public bool IsFixed;

        [JsonProperty("doesNotCountAsHit")]
        public bool DoesNotCountAsHit;

        [JsonProperty("increaseCriticalHitChance")]
        public double? IncreaseCriticalHitChance;

        [JsonProperty("valueCapExpression")]
        public string ValueCapExpression;
    }

    public class HealParamsStub
    {
        [JsonProperty("canBeCritical")]
        public bool CanBeCritical;
    }

    public class EvenParamsStub
    {
        [JsonProperty("evenMode")]
        public EvenMode EvenMode;
    }

    public class ChangeStatParamsStub
    {
        [JsonProperty("statKindId")]
        public StatKindId Param;
    }

    public class ActivateSkillParamsStub
    {
        [JsonProperty("skillIndex")]
        public int SkillIndex;

        [JsonProperty("skillOwner")]
        public ActivateSkillOwner SkillOwner;

        [JsonProperty("targetExpression")]
        public string TargetExpression;
    }

    public class ShowHiddenSkillParamsStub
    {
        [JsonProperty("skillTypeId")]
        public int SkillTypeId;

        [JsonProperty("shouldHide")]
        public bool ShouldHide;
    }

    public class ChangeSkillCooldownParamsStub
    {
        [JsonProperty("turns")]
        public int Turns;

        [JsonProperty("skillIndex")]
        public int SkillIndex;

        [JsonProperty("isRandomSkill")]
        public bool IsRandomSkill;

        [JsonProperty("skillToChange")]
        public SkillToChange SkillToChange;
    }

    public class ChangeEffectLifetimeParamsStub
    {
        [JsonProperty("type")]
        public AppliedEffectType Type;

        [JsonProperty("turns")]
        public int Turns;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId> EffectTypeIds;
    }

    public class ShareDamageParamsStub
    {
        [JsonProperty("targetDamageCutPerc")]
        public double TargetDamageCutPerc;

        [JsonProperty("transferedDamagePerc")]
        public double TransferedDamagePerc;

        [JsonProperty("defenceModifier")]
        public double? DefenceModifier;
    }

    public class BlockEffectParamsStub
    {
        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<int> EffectTypeIds;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<int> EffectKindIds;

        [JsonProperty("blockGuaranteed")]
        public bool BlockGuaranteed;
    }

    public class TeamAttackParamsStub
    {
        public int TeammatesCount;

        public bool ExcludeProducerFromAttack;

        public IReadOnlyList<int> PreferredHeroTypes;

        public bool AlwaysUsePreferredWhenPossible;

        public string AllySelectorExpression;
    }

    public class SummonParamsStub
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

    public class DestroyHpParamsStub
    {
        [JsonProperty("ignoreShield")]
        public bool IgnoreShield;
    }

    public class ReviveParamsStub
    {
        [JsonProperty("healPercent")]
        public double HealPercent;

        [JsonProperty("ignoreBlockRevive")]
        public bool IgnoreBlockRevive;
    }

    public class CounterattackParamsStub
    {
        [JsonProperty("skillIndex")]
        public int SkillIndex;

        [JsonProperty("noPenalty")]
        public bool NoPenalty;
    }

    public class ForceStatusEffectTickParamsStub
    {
        [JsonProperty("ticks")]
        public int Ticks;

        [JsonProperty("effectTypeIds")]
        public IReadOnlyList<StatusEffectTypeId> EffectTypeIds;

        [JsonProperty("effectCount")]
        public int EffectCount;
    }

    public class CrabShellLayerStub
    {
        [JsonProperty("type")]
        public CrabShellLayerType Type;

        [JsonProperty("multiplierFormula")]
        public string MultiplierFormula;

        [JsonProperty("conditionFormula")]
        public string ConditionFormula;
    }

    public class CrabShellParamsStub
    {
        [JsonProperty("layers")]
        public IReadOnlyList<CrabShellLayerStub> Layers;
    }

    public class ReturnDebuffsParamsStub
    {
        [JsonProperty("applyMode")]
        public ApplyMode ApplyMode;
    }

    public class HitTypeParamsStub
    {
        [JsonProperty("hitTypeToChange")]
        public HitType HitTypeToChange;

        [JsonProperty("hitType")]
        public HitType HitType;
    }

    public class PassiveBonusParamsStub
    {
        [JsonProperty("bonus")]
        public PassiveBonus Bonus;
    }

    public class MultiplyStatusEffectParamsStub
    {
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("turnsModifier")]
        public int TurnsModifier;

        [JsonProperty("effectKindIds")]
        public IReadOnlyList<EffectKindId> EffectKindIds;

        [JsonProperty("targetSelectorExpression")]
        public string TargetSelectorExpression;
    }

    public class IgnoreProtectionEffectsParamsStub
    {
        [JsonProperty("ignoreBlockDamage")]
        public bool IgnoreBlockDamage;

        [JsonProperty("ignoreShield")]
        public bool IgnoreShield;

        [JsonProperty("ignoreUnkillable")]
        public bool IgnoreUnkillable;
    }

    public class ChangeEffectTargetParamsStub
    {
        [JsonProperty("overrideApplyMode")]
        public bool OverrideApplyMode;

        [JsonProperty("applyMode")]
        public ApplyMode ApplyMode;
    }

    public class EffectType
    {
        [JsonProperty("id")]
        public int TypeId;

        [JsonProperty("count")]
        public int Count;

        [JsonProperty("multiplier")]
        public string Multiplier;

        [JsonProperty("stack")]
        public int StackCount;

        [JsonProperty("kindId")]
        public EffectKindId KindId;

        // TODO: there's a lot more data here we could extract

        [JsonProperty("group")]
        public EffectGroup Group;

        [JsonProperty("targetParams")]
        public TargetParamsStub TargetParams;

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
        public EffectRelationStub Relation;

        [JsonProperty("condition")]
        public string Condition;

        [JsonProperty("chance")]
        public double? Chance;

        [JsonProperty("repeatChance")]
        public double? RepeatChance;

        [JsonProperty("statusEffectParams")]
        public StatusEffectParamsStub StatusParams;

        [JsonProperty("valueCap")]
        public string ValueCap;

        [JsonProperty("applyInstantEffectMode")]
        public ApplyMode ApplyInstantEffectMode;

        [JsonProperty("persistsThroughRounds")]
        public bool PersistsThroughRounds;

        [JsonProperty("snapshotRequired")]
        public bool SnapshotRequired;

        [JsonProperty("ignoredEffects")]
        public IReadOnlyList<EffectKindId> IgnoredEffects;

        [JsonProperty("applyStatusEffectParams")]
        public ApplyStatusEffectParamsStub ApplyStatusEffectParams;

        [JsonProperty("unapplyStatusEffectParams")]
        public UnapplyStatusEffectParamsStub UnapplyStatusEffectParams;

        [JsonProperty("transferDebuffParams")]
        public TransferDebuffParamsStub TransferDebuffParams;

        [JsonProperty("damageParams")]
        public DamageParamsStub DamageParams;

        [JsonProperty("healParams")]
        public HealParamsStub HealParams;

        [JsonProperty("evenParams")]
        public EvenParamsStub EvenParams;

        [JsonProperty("changeStatParams")]
        public ChangeStatParamsStub ChangeStatParams;

        [JsonProperty("activateSkillParams")]
        public ActivateSkillParamsStub ActivateSkillParams;

        [JsonProperty("showHiddenSkillParams")]
        public ShowHiddenSkillParamsStub ShowHiddenSkillParams;

        [JsonProperty("changeSkillCooldownParams")]
        public ChangeSkillCooldownParamsStub ChangeSkillCooldownParams;

        [JsonProperty("changeEffectLifetimeParams")]
        public ChangeEffectLifetimeParamsStub ChangeEffectLifetimeParams;

        [JsonProperty("shareDamageParams")]
        public ShareDamageParamsStub ShareDamageParams;

        [JsonProperty("blockEffectParams")]
        public BlockEffectParamsStub BlockEffectParams;

        [JsonProperty("summonParams")]
        public SummonParamsStub SummonParams;

        [JsonProperty("teamAttackParams")]
        public TeamAttackParamsStub TeamAttackParams;

        [JsonProperty("destroyHpParams")]
        public DestroyHpParamsStub DestroyHpParams;

        [JsonProperty("reviveParams")]
        public ReviveParamsStub ReviveParams;

        [JsonProperty("counterattackParams")]
        public CounterattackParamsStub CounterattackParams;

        [JsonProperty("forceTickParams")]
        public ForceStatusEffectTickParamsStub ForceTickParams;

        [JsonProperty("crabShellParams")]
        public CrabShellParamsStub CrabShellParams;

        [JsonProperty("returnDebuffsParams")]
        public ReturnDebuffsParamsStub ReturnDebuffsParams;

        [JsonProperty("hitTypeParams")]
        public HitTypeParamsStub HitTypeParams;

        [JsonProperty("passiveBonusParams")]
        public PassiveBonusParamsStub PassiveBonusParams;

        [JsonProperty("multiplyStatusEffectParams")]
        public MultiplyStatusEffectParamsStub MultiplyStatusEffectParams;

        [JsonProperty("ignoreProtectionEffectsParams")]
        public IgnoreProtectionEffectsParamsStub IgnoreProtectionEffectsParams;

        [JsonProperty("changeEffectTargetParams")]
        public ChangeEffectTargetParamsStub ChangeEffectTargetParams;

        [JsonProperty("multiplierDependsOnRelation")]
        public bool MultiplierDependsOnRelation;
    }

    public class SkillUpgrade
    {
        [JsonProperty("type")]
        public string SkillBonusType;

        [JsonProperty("value")]
        public double Value;
    }


    public enum BattlePhaseId
    {
        Unknown = 0,
        BattleStarted = 10,
        BattleFinished = 11,
        BeforeTurnStarted = 20,
        AfterTurnStarted = 21,
        TurnFinished = 22,
        RoundStarted = 30,
        RoundFinished = 31,
        ImmunitiesProcessing = 32,
        BeforeEffectProcessed = 40,
        BeforeEffectProcessedOnTarget = 41,
        BeforeEffectAppliedOnTarget = 42,
        AfterEffectAppliedOnTarget = 43,
        AfterEffectProcessedOnTarget = 44,
        AfterEffectProcessed = 45,
        EffectExpired = 46,
        BeforeEffectChanceRolling = 47,
        AfterEffectChanceRolling = 48,
        TargetContextHasJustBeenCreated = 49,
        BeforeDamageCalculated = 50,
        AfterDamageCalculated = 51,
        BeforeDamageDealt = 52,
        AfterDamageDealt = 53,
        BlockDamageProcessing = 54,
        AfterDamageContextCreated = 55,
        AfterHealthReduced = 56,
        AfterHitTypeCalculated = 57,
        UnkillableProcessing = 58,
        CocoonProcessing = 59,
        BeforeHealDealt = 60,
        AfterHealDealt = 61,
        BeforeHealCalculated = 62,
        AllHeroesDeathProcessed = 70,
        HeroDead = 71,
        AfterSkillEffectsProcessed = 72,
        AfterHeroSummoned = 80,
        BeforeAppliedEffectsUpdate = 100,
        FearProcessing = 111,
        AfterSkillUsed = 112,
        AfterAllSkillsUsed = 113,
        BeforeSkillProcessed = 114,
        AfterStatusEffectToApplySelected = 120,
        CancelEffectProcessing = 130,
        BeforeEffectUnappliedFromHero = 140,
        AfterEffectUnappliedFromHero = 141,
        BeforeDestroyHpDealt = 150,
        CrabShellProcessing = 160,
        BeforeStaminaChanged = 170,
        StatusReviveOnDeathProcessing = 180,
        PassiveReviveOnDeathProcessing = 181,
        EffectBlocked = 190,
        StoneSkinAbsorptionProcessing = 200,
        AfterHeroDevoured = 210,
        DigestionAbsorptionProcessing = 220,
        HydraHeadGrown = 230
    }
}
