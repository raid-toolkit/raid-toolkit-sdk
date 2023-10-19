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

export enum SkillTargets {
  Producer,
  AliveAllies,
  AliveEnemies,
  DeadAllies,
  DeadEnemies,
  AllAllies,
  AllEnemies,
  AliveAlliesExceptProducer,
  AliveEnemiesIncludeInvisible,
  AllAlliesExceptProducer,
}

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
  visibility: Visibility;
  unblockable: boolean;
  effects: EffectType[];
  upgrades: SkillUpgrade[];
  doesDamage: boolean;
  targets?: SkillTargets;
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
  targetType: EffectTargetType;
  exclusion: TargetExclusion;
  exclusive: boolean;
  firstHitInSelected: boolean;
  condition: string;
}

export interface EffectRelation {
  effectTypeId: StatusEffectTypeId;
  effectKindIds: EffectKindId[];
  effectKindGroups: EffectKindGroup[];
  phase: number[];
  activateOnGlancingHit: boolean;
}

export interface StatusEffectParams {
  strengthInFamily: number;
  forcedTickAllowed: boolean;
  lifetimeUpdateType: LifetimeUpdateType;
  unapplyWhenProducerDied: boolean;
}

export interface StatusEffectInfo {
  typeId: StatusEffectTypeId;
  duration: number;
  ignoreEffectsLimit: boolean;
  applyMode: ApplyMode;
  protection: Protection;
}

export interface Protection {
  mode: ProtectionMode;
  chance?: number;
}

export interface ApplyStatusEffectParams {
  statusEffectInfos: StatusEffectInfo[];
}

export interface UnapplyStatusEffectParams {
  count: number;
  statusEffectTypeIds: StatusEffectTypeId[];
  unapplyMode: UnapplyEffectMode;
  removeFrom: UnapplyEffectTarget;
  applyTo: UnapplyEffectTarget;
}

export interface TransferDebuffParams {
  count: number;
  statusEffectTypeIds: StatusEffectTypeId[];
  unapplyMode: UnapplyEffectMode;
  includeProducer: boolean;
  applyMode: ApplyMode;
  removeFrom: UnapplyEffectTarget;
  applyTo: UnapplyEffectTarget;
}

export interface DamageParams {
  hitType: HitType;
  elementRelation: ElementRelation;
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
  evenMode: EvenMode;
}

export interface ChangeStatParams {
  statKindId: StatKindId;
}

export interface ActivateSkillParams {
  skillIndex: number;
  skillOwner: ActivateSkillOwner;
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
  skillToChange: SkillToChange;
}

export interface ChangeEffectLifetimeParams {
  type: AppliedEffectType;
  turns: number;
  count: number;
  effectTypeIds: StatusEffectTypeId[];
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
  effectTypeIds: StatusEffectTypeId[];
  effectCount: number;
}

export interface CrabShellLayer {
  type: CrabShellLayerType;
  multiplierFormula: string;
  conditionFormula: string;
}

export interface CrabShellParams {
  layers: CrabShellLayer[];
}

export interface ReturnDebuffsParams {
  applyMode: ApplyMode;
}

export interface HitTypeParams {
  hitTypeToChange: HitType;
  hitType: HitType;
}

export interface PassiveBonusParams {
  bonus: PassiveBonus;
}

export interface MultiplyStatusEffectParams {
  count: number;
  turnsModifier: number;
  effectKindIds: EffectKindId[];
  targetSelectorExpression: string;
}

export interface IgnoreProtectionEffectsParams {
  ignoreBlockDamage: boolean;
  ignoreShield: boolean;
  ignoreUnkillable: boolean;
}

export interface ChangeEffectTargetParams {
  overrideApplyMode: boolean;
  applyMode: ApplyMode;
}

export interface EffectType {
  id: number;
  count: number;
  multiplier: string;
  stack: number;
  kindId: EffectKindId;
  group?: EffectGroup;
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
  applyInstantEffectMode?: ApplyMode;
  persistsThroughRounds: boolean;
  snapshotRequired: boolean;
  ignoredEffects: EffectKindId[];
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
  type: SkillBonusType;
  value: number;
}
