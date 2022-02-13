import { DifficultyId } from './Enums';
import { LocalizedText } from './HeroType';

export interface AreaData {
  areaId: number;
}

export interface RegionData extends AreaData {
  regionId: number;
}

export interface StageData extends RegionData {
  stageId: number;
  difficulty: DifficultyId;
  bossName: LocalizedText;
}
