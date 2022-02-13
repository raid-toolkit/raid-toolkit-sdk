import { Stats } from './HeroType';

export enum ArenaLeagueId {
  None = 'None',
  BronzeI = 'BronzeI',
  BronzeII = 'BronzeII',
  BronzeIII = 'BronzeIII',
  BronzeIV = 'BronzeIV',
  SilverI = 'SilverI',
  SilverII = 'SilverII',
  SilverIII = 'SilverIII',
  SilverIV = 'SilverIV',
  GoldI = 'GoldI',
  GoldII = 'GoldII',
  GoldIII = 'GoldIII',
  GoldIV = 'GoldIV',
  Platinum = 'Platinum',
}

export interface ArenaLeague {
  id: ArenaLeagueId;
  statBonus: Stats;
}
