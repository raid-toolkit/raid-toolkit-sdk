import { ApiDefinition, methodStub } from '@remote-ioc/runtime';
import { ArenaLeague, ArenaLeagueId } from '../Types/Arena';
import { ArtifactSetKind, ArtifactSetKindId } from '../Types/Artifacts';
import { HeroType } from '../Types/HeroType';
import { SkillType } from '../Types/SkillType';
import { AreaData, RegionData, StageData } from '../Types/Stages';

export interface StaticData {
  localizedStrings: Record<string, string>;
  heroTypes: Record<number, HeroType>;
  skillTypes: Record<number, SkillType>;
  stageData: {
    areas: Record<string, AreaData>;
    regions: Record<string, RegionData>;
    stages: Record<string, StageData>;
  };
  arenaData: { leagues: Record<ArenaLeagueId, ArenaLeague> };
  artifactData: { setKinds: Record<ArtifactSetKindId, ArtifactSetKind> };
}
@ApiDefinition('static-data')
export class IStaticDataApi {
  public async getAllData(): Promise<unknown> {
    methodStub(this);
  }
  public async getLocalizedStrings(): Promise<StaticData['localizedStrings']> {
    methodStub(this);
  }
  public async getArenaData(): Promise<Pick<StaticData, 'arenaData'>> {
    methodStub(this);
  }
  public async getArtifactData(): Promise<Pick<StaticData, 'artifactData'>> {
    methodStub(this);
  }
  public async getHeroData(): Promise<Pick<StaticData, 'heroTypes'>> {
    methodStub(this);
  }
  public async getSkillData(): Promise<Pick<StaticData, 'skillTypes'>> {
    methodStub(this);
  }
  public async getStageData(): Promise<Pick<StaticData, 'stageData'>> {
    methodStub(this);
  }
}
