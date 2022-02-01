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
  affinity: Affinity;
  faction: Faction;
  role: HeroRole;
  rarity: Rarity;
  ascended: number;
  modelName: string;
  avatarKey: string;
  leaderSkill: LeaderStatBonus;
  skillTypeIds: number[];
  unscaledStats: Stats;
  brain: string;
}
