export enum EffectGroup {
  Active,
  Passive,
}

export enum EffectKindId {
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
export enum EffectTargetType {
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

export enum TargetExclusion {
  Target,
  Producer,
  RelationTarget,
  RelationProducer,
}

export enum StatusEffectTypeId {
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
  ArtifactSet_Shield = 370, // 0x00000172
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

export enum EffectKindGroup {
  Undefined = 1,
  StatusDebuff = 2,
  ResistibleInstantDebuff = 3,
  EffectThatApplyStatusDebuffs = 4,
}

export enum LifetimeUpdateType {
  OnStartTurn,
  OnEndTurn,
  Custom,
}

export enum ApplyMode {
  Unresistable,
  Guaranteed,
}

export enum ProtectionMode {
  ProtectedFromEnemies,
  FullyProtected,
}

export enum UnapplyEffectTarget {
  Target = 1,
  Producer = 2,
}

export enum UnapplyEffectMode {
  Selected,
  AllExceptSelected,
}

export enum HitType {
  Normal,
  Crushing,
  Critical,
  Glancing,
}

export enum ElementRelation {
  Disadvantage = -1, // 0xFFFFFFFF
  Neutral = 0,
  Advantage = 1,
}

export enum EvenMode {
  Average,
  Highest,
  Lowest,
}

export enum ActivateSkillOwner {
  Producer,
  Target,
}

export enum SkillToChange {
  Random = 1,
  ByIndex = 2,
  SkillOfCurrentContext = 3,
  All = 4,
}

export enum AppliedEffectType {
  Buff,
  Debuff,
}

export enum CrabShellLayerType {
  First,
  Second,
  Third,
}

export enum PassiveBonus {
  Heal = 1,
  ShieldCreation = 2,
  StaminaRecovery = 3,
  ArtifactSetStats = 4,
}

export enum SkillBonusType {
  Attack = 'Attack',
  Health = 'Health',
  EffectChance = 'EffectChance',
  CooltimeTurn = 'CooltimeTurn',
  ShieldCreation = 'ShieldCreation',
}
