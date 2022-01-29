import { StatKindId } from './Enums';
import { LocalizedText } from './HeroType';
import {
  ActivateSkillOwner,
  AppliedEffectType,
  ApplyMode,
  CrabShellLayerType,
  EffectGroup,
  EffectKindGroup,
  EffectKindId,
  EffectTargetType,
  ElementRelation,
  EvenMode,
  HitType,
  LifetimeUpdateType,
  PassiveBonus,
  ProtectionMode,
  SkillBonusType,
  SkillToChange,
  StatusEffectTypeId,
  TargetExclusion,
  UnapplyEffectMode,
  UnapplyEffectTarget,
} from './SkillEnums';

export interface Skill {
  typeId: number;
  id: number;
  level: number;
}

export enum Visibility {
  Visible = 0,
  HiddenOnHud = 1,
  HiddenOnHudWithVisualization = 2,
  HiddenOnHudVisibleForAl = 3,
  HiddenOnHudWithVisualisationVisibleForAI = 4,
}

export interface BaseSkillData {
  typeId: number;
  cooldown: number;
  visibility: keyof typeof Visibility;
  unblockable: boolean;
  effects: EffectType[];
  upgrades: SkillUpgrade[];
  doesDamage: boolean;
}

export interface SkillType extends BaseSkillData {
  name: LocalizedText;
  description: LocalizedText;
}

export interface SkillSnapshhot extends BaseSkillData {
  name: string;
  description: string;
  level: number;
}

export interface TargetParams {
  targetType: keyof typeof EffectTargetType;
  exclusion: keyof typeof TargetExclusion;
  exclusive: boolean;
  firstHitInSelected: boolean;
  condition: string;
}

export interface EffectRelation {
  effectTypeId: StatusEffectTypeId;
  effectKindIds: keyof typeof EffectKindId[];
  EffectKindGroups: keyof typeof EffectKindGroup[];
  phase: string;
  activateOnGlancingHit: boolean;
}

export interface StatusEffectParams {
  strengthInFamily: number;
  forcedTickAllowed: boolean;
  lifetimeUpdateType: keyof typeof LifetimeUpdateType;
  unapplyWhenProducerDied: boolean;
}

export interface StatusEffectInfo {
  typeId: StatusEffectTypeId;
  duration: number;
  ignoreEffectsLimit: boolean;
  applyMode: keyof typeof ApplyMode;
  protection: Protection;
}

export interface Protection {
  mode: keyof typeof ProtectionMode;
  chance?: number;
}

export interface ApplyStatusEffectParams {
  statusEffectInfos: StatusEffectInfo[];
}

export interface UnapplyStatusEffectParams {
  count: number;
  statusEffectTypeIds: keyof typeof StatusEffectTypeId[];
  unapplyMode: keyof typeof UnapplyEffectMode;
  removeFrom: keyof typeof UnapplyEffectTarget;
  applyTo: keyof typeof UnapplyEffectTarget;
}

export interface TransferDebuffParams {
  count: number;
  statusEffectTypeIds: keyof typeof StatusEffectTypeId[];
  unapplyMode: keyof typeof UnapplyEffectMode;
  includeProducer: boolean;
  applyMode: keyof typeof ApplyMode;
  removeFrom: keyof typeof UnapplyEffectTarget;
  applyTo: keyof typeof UnapplyEffectTarget;
}

export interface DamageParams {
  hitType: keyof typeof HitType;
  elementRelation: keyof typeof ElementRelation;
  defenceModifier: number;
  isFixed: boolean;
  doesNotCountAsHit: boolean;
  increaseCriticalHitChance: boolean;
  valueCapExpression: boolean;
}

export interface HealParams {
  canBeCritical: boolean;
}

export interface EvenParams {
  evenMode: keyof typeof EvenMode;
}

export interface ChangeStatParams {
  statKindId: keyof typeof StatKindId;
}

export interface ActivateSkillParams {
  skillIndex: number;
  skillOwner: keyof typeof ActivateSkillOwner;
  targetExpression: string;
}

export interface ShowHiddenSkillParams {
  skillTypeId: number;
  shouldHide: boolean;
}

export interface ChangeSkillCooldownParams {
  turns: number;
  skillIndex: number;
  isRandomSkill: boolean;
  skillToChange: keyof typeof SkillToChange;
}

export interface ChangeEffectLifetimeParams {
  type: keyof typeof AppliedEffectType;
  turns: number;
  count: number;
  effectTypeIds: keyof typeof StatusEffectTypeId[];
}

export interface ShareDamageParams {
  targetDamageCutPerc: number;
  transferredDamagePerc: number;
  defenceModifier: number;
}

export interface BlockEffectParams {
  effectTypeIds: StatusEffectTypeId[];
  effectKindIds: EffectKindId[];
  blockGuaranteed: boolean;
}

export interface TeamAttackParams {
  TeammatesCount: number;
  ExcludeProducerFromAttack: boolean;
  PreferredHeroTypes: number[];
  AlwaysUsePreferredWhenPossible: boolean;
  AllySelectorExpression: string;
}

export interface SummonParams {
  baseTypeId: number;
  ascendLevelFormula: string;
  gradeFormula: string;
  levelFormual: string;
  removeAfterDeath: boolean;
  slotsLimit: number;
}

export interface DestroyHpParams {
  ignoreShield: boolean;
}

export interface ReviveParams {
  healPercent: number;
  ignoreBlockRevive: boolean;
}

export interface CounterattackParams {
  skillIndex: number;
  noPenalty: boolean;
}

export interface ForceStatusEffectTickParams {
  ticks: number;
  effectTypeIds: keyof typeof StatusEffectTypeId[];
  effectCount: number;
}

export interface CrabShellLayer {
  type: keyof typeof CrabShellLayerType;
  multiplierFormula: string;
  conditionFormula: string;
}

export interface CrabShellParams {
  layers: CrabShellLayer[];
}

export interface ReturnDebuffsParams {
  applyMode: keyof typeof ApplyMode;
}

export interface HitTypeParams {
  hitTypeToChange: keyof typeof HitType;
  hitType: keyof typeof HitType;
}

export interface PassiveBonusParams {
  bonus: keyof typeof PassiveBonus;
}

export interface MultiplyStatusEffectParams {
  count: number;
  turnsModifier: number;
  effectKindIds: keyof typeof EffectKindId[];
  targetSelectorExpression: string;
}

export interface IgnoreProtectionEffectsParams {
  ignoreBlockDamage: boolean;
  ignoreShield: boolean;
  ignoreUnkillable: boolean;
}

export interface ChangeEffectTargetParams {
  overrideApplyMode: boolean;
  applyMode: keyof typeof ApplyMode;
}

export interface EffectType {
  id: number;
  count: number;
  multiplier: string;
  stack: number;
  kindId: keyof typeof EffectKindId;

  group?: keyof typeof EffectGroup;
  targetParams?: TargetParams;
  isEffectDescription: boolean;
  considersDead: boolean;
  leaveThroughDeath: boolean;
  doesntSetSkillOnCooldown: boolean;
  ignoresCooldown: boolean;
  isUnique: boolean;
  iterationChanceRolling: boolean;
  relation?: EffectRelation;
  condition: string;
  chance: number;
  repeatChance: number;
  statusEffectParams?: StatusEffectParams;
  valueCap: string;
  applyInstantEffectMode?: keyof typeof ApplyMode;
  persistsThroughRounds: boolean;
  snapshotRequired: boolean;
  ignoredEffects: keyof typeof EffectKindId[];
  applyStatusEffectParams?: ApplyStatusEffectParams;
  unapplyStatusEffectParams?: UnapplyStatusEffectParams;
  transferDebuffParams?: TransferDebuffParams;
  damageParams?: DamageParams;
  healParams?: HealParams;
  evenParams?: EvenParams;
  changeStatParams?: ChangeStatParams;
  activateSkillParams?: ActivateSkillParams;
  showHiddenSkillParams?: ShowHiddenSkillParams;
  changeSkillCooldownParams?: ChangeSkillCooldownParams;
  changeEffectLifetimeParams?: ChangeEffectLifetimeParams;
  shareDamageParams?: ShareDamageParams;
  blockEffectParams?: BlockEffectParams;
  summonParams?: SummonParams;
  teamAttackParams?: TeamAttackParams;
  destroyHpParams?: DestroyHpParams;
  reviveParams?: ReviveParams;
  counterattackParams?: CounterattackParams;
  forceTickParams?: ForceStatusEffectTickParams;
  crabShellParams?: CrabShellParams;
  returnDebuffsParams?: ReturnDebuffsParams;
  hitTypeParams?: HitTypeParams;
  passiveBonusParams?: PassiveBonusParams;
  multiplyStatusEffectParams?: MultiplyStatusEffectParams;
  ignoreProtectionEffectsParams?: IgnoreProtectionEffectsParams;
  changeEffectTargetParams?: ChangeEffectTargetParams;
  multiplierDependsOnRelation: boolean;
}

export interface SkillUpgrade {
  type: keyof typeof SkillBonusType;
  value: number;
}
