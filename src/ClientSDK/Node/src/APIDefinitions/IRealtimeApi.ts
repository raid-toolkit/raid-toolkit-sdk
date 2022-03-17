import { ApiDefinition, methodStub } from "@remote-ioc/runtime";
import { AccountInfo, TypedEventEmitter } from "../Types";

@ApiDefinition("realtime-api")
export class IRealtimeApi extends TypedEventEmitter<{ updated: [string] }> {
  public async getConnectedAccounts(): Promise<AccountInfo[]> {
    methodStub(this);
  }

  public async getLastBattleResponse(accountId: string): Promise<unknown> {
    methodStub(this, accountId);
  }
}
