import { ApiDefinition, methodStub } from '@remote-ioc/runtime';

@ApiDefinition('static-data')
export class IStaticDataApi {
  public async getAllData(): Promise<unknown> {
    methodStub(this);
  }
  public async getLocalizedStrings(): Promise<unknown> {
    methodStub(this);
  }
  public async getArenaData(): Promise<unknown> {
    methodStub(this);
  }
  public async getArtifactData(): Promise<unknown> {
    methodStub(this);
  }
  public async getHeroData(): Promise<unknown> {
    methodStub(this);
  }
  public async getSkillData(): Promise<unknown> {
    methodStub(this);
  }
  public async getStageData(): Promise<unknown> {
    methodStub(this);
  }
}
