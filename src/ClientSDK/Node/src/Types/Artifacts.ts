import { LocalizedText, StatBonus } from './HeroType';

export enum ArtifactSetKindId {
  None = 'None',
  Hp = 'Hp',
  AttackPower = 'AttackPower',
  Defense = 'Defense',
  AttackSpeed = 'AttackSpeed',
  CriticalChance = 'CriticalChance',
  CriticalDamage = 'CriticalDamage',
  Accuracy = 'Accuracy',
  Resistance = 'Resistance',
  LifeDrain = 'LifeDrain',
  DamageIncreaseOnHpDecrease = 'DamageIncreaseOnHpDecrease',
  SleepChance = 'SleepChance',
  BlockHealChance = 'BlockHealChance',
  FreezeRateOnDamageReceived = 'FreezeRateOnDamageReceived',
  Stamina = 'Stamina',
  Heal = 'Heal',
  BlockDebuff = 'BlockDebuff',
  Shield = 'Shield',
  GetExtraTurn = 'GetExtraTurn',
  IgnoreDefense = 'IgnoreDefense',
  DecreaseMaxHp = 'DecreaseMaxHp',
  StunChance = 'StunChance',
  DotRate = 'DotRate',
  ProvokeChance = 'ProvokeChance',
  Counterattack = 'Counterattack',
  CounterattackOnCrit = 'CounterattackOnCrit',
  AoeDamageDecrease = 'AoeDamageDecrease',
  CooldownReductionChance = 'CooldownReductionChance',
  CriticalHealMultiplier = 'CriticalHealMultiplier',
  AttackPowerAndIgnoreDefense = 'AttackPowerAndIgnoreDefense',
  HpAndHeal = 'HpAndHeal',
  ShieldAndAttackPower = 'ShieldAndAttackPower',
  ShieldAndCriticalChance = 'ShieldAndCriticalChance',
  ShieldAndHp = 'ShieldAndHp',
  ShieldAndSpeed = 'ShieldAndSpeed',
  UnkillableAndSpdAndCrDmg = 'UnkillableAndSpdAndCrDmg',
  BlockReflectDebuffAndHpAndDef = 'BlockReflectDebuffAndHpAndDef',
  HpAndDefence = 'HpAndDefence',
  AccuracyAndSpeed = 'AccuracyAndSpeed',
  CritDmgAndTransformWeekIntoCritHit = 'CritDmgAndTransformWeekIntoCritHit',
  ResistanceAndBlockDebuff = 'ResistanceAndBlockDebuff',
  AttackAndCritRate = 'AttackAndCritRate',
  FreezeResistAndRate = 'FreezeResistAndRate',
  CritRateAndLifeDrain = 'CritRateAndLifeDrain',
  PassiveShareDamageAndHeal = 'PassiveShareDamageAndHeal',
  ResistAndDef = 'ResistAndDef',
  CritRateAndIgnoreDefMultiplier = 'CritRateAndIgnoreDefMultiplier',
  BuffChanceResHpSpd = 'BuffChanceResHpSpd',
  StoneSkinHpResDef = 'StoneSkinHpResDef',
  IgnoreCooldown = 'IgnoreCooldown',
  RemoveDebuff = 'RemoveDebuff',
  ShieldAccessory = 'ShieldAccessory',
  ChangeHitType = 'ChangeHitType',
  CounterattackAccessory = 'CounterattackAccessory',
}

export interface ArtifactSetKind {
  setKindId: ArtifactSetKindId;
  artifactCount: number;
  name: LocalizedText;
  statBonuses: StatBonus[];
  skillBonus?: number;
  shortDescription: LocalizedText;
  longDescription: LocalizedText;
}
