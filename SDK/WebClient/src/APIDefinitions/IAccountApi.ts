import { ApiDefinition, methodStub } from '@remote-ioc/runtime';
import { AccountInfo, TypedEventEmitter } from '../Types';

@ApiDefinition('account-api')
export class IAccountApi extends TypedEventEmitter<{ updated: [string] }> {
  public async getAccounts(): Promise<AccountInfo[]> {
    methodStub(this);
  }

  public async getAccountDump(accountId: string): Promise<{}> {
    methodStub(this, accountId);
  }

  public async getArtifacts(accountId: string): Promise<unknown[]> {
    methodStub(this, accountId);
  }

  public async getArtifactById(accountId: string, artifactId: number): Promise<unknown> {
    methodStub(this, accountId, artifactId);
  }

  public async getHeroes(accountId: string, snapshot?: boolean): Promise<[]> {
    methodStub(this, accountId, snapshot);
  }

  public async getHeroById(accountId: string, heroId: number, snapshot?: boolean): Promise<unknown> {
    methodStub(this, accountId, heroId, snapshot);
  }

  public async getAllResources(accountId: string): Promise<unknown> {
    methodStub(this, accountId);
  }

  public async getArena(accountId: string): Promise<unknown> {
    methodStub(this, accountId);
  }

  public async getAcademy(accountId: string): Promise<unknown> {
    methodStub(this, accountId);
  }
}
