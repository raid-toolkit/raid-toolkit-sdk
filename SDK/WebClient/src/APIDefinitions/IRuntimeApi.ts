import { ApiDefinition, methodStub } from '@remote-ioc/runtime';
import { AccountInfo, TypedEventEmitter } from '../Types';

@ApiDefinition('runtime-api')
export class IRuntimeApi extends TypedEventEmitter<{ updated: [string] }> {
  public async getConnectedAccounts(): Promise<AccountInfo[]> {
    methodStub(this);
  }

  public async getLastBattleResponse(accountId: string): Promise<unknown> {
    methodStub(this, accountId);
  }
}
