import { ViewKey } from './ViewKey';

export interface ViewInfo {
  viewId: ViewKey;
  viewKey: keyof typeof ViewKey;
}

export interface GivenDamage {
  demonLord: number | null;
  hydra: number | null;
}

export interface LastBattleDataObject {
  battleKindId: string;
  heroesExperience: number;
  heroesExperienceAdded: number;
  turnCount: number | null;
  givenDamage: GivenDamage;
  tournamentPointsByStateId: { [key: number]: number };
  masteryPointsByHeroId: { [key: number]: { [key: string]: number } };
}
