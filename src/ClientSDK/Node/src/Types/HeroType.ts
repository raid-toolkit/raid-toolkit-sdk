import { Affinity, Faction, HeroRole, Rarity } from './Enums';

export interface LocalizedText {
  key: string;
  defaultValue: string;
  localizedValue: string;
}

export interface Stats {
  Health: number;
  Attack: number;
  Defense: number;
  Speed: number;
  Resistance: number;
  Accuracy: number;
  CriticalChance: number;
  CriticalDamage: number;
  CriticalHeal: number;
}

export interface StatBonus {
  kind: string;
  absolute: boolean;
  value: number;
}

export interface LeaderStatBonus extends StatBonus {
  areaTypeId: string;
  affinity: Affinity;
}

export interface HeroType {
  typeId: number;
  name: LocalizedText;
  shortName: LocalizedText;
  affinity: Affinity;
  faction: Faction;
  rarity: Rarity;
  ascended: number;
  leaderSkill: LeaderStatBonus;
  brain: string;
  forms: HeroForm[];
}

export interface HeroVisualInfo {
  avatar: string;
  model: string;
  showcase: string;
}

export interface HeroForm {
  role: HeroRole;
  skillTypeIds: number[];
  unscaledStats: Stats;
  visualInfosBySkin: Record<number, HeroVisualInfo>;
}
