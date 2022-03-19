import { ApiDefinition, methodStub } from '@remote-ioc/runtime';
import { AccountInfo, LastBattleDataObject, TypedEventEmitter, ViewInfo } from '../Types';

@ApiDefinition('realtime-api')
export class IRealtimeApi extends TypedEventEmitter<{
  'account-list-updated': [string];
  'view-changed': [accountId: string];
  'last-battle-response-updated': [accountId: string];
}> {
  public async getConnectedAccounts(): Promise<AccountInfo[]> {
    methodStub(this);
  }

  public async getLastBattleResponse(accountId: string): Promise<LastBattleDataObject> {
    methodStub(this, accountId);
  }

  public async getCurrentViewInfo(accountId: string): Promise<ViewInfo> {
    methodStub(this, accountId);
  }
}
