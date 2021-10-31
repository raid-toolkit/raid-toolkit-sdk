import { ApiDefinition, methodStub } from "@remote-ioc/runtime";
import { AccountInfo } from "../Types";

@ApiDefinition("account-api")
export class IAccountApi {
  public async getAccounts(): Promise<AccountInfo[]> {
    methodStub(this);
  }
  public async getAccountDump(accountId: string): Promise<{}> {
    methodStub(this, accountId);
  }
}
