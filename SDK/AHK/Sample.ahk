#include <RaidToolkit>
#Include, <jxon>

class TestClass
{
    Client := new RaidToolkitClient()

    Run()
    {
        this.Client.AccountApi.GetAccounts().Then(ObjBindMethod(this, "_HandleGetAccountsResult"))
    }

    _DumpData(name, result)
    {
        OutputDebug, % name . ":" . Jxon_Dump(result) . "`n"
        return result
    }

    _HandleError(name, result)
    {
        OutputDebug, "***ERROR***"
        OutputDebug, % name . ":" . Jxon_Dump(result) . "`n"
        OutputDebug, "***ERROR***"
    }

    _HandleGetAccountsResult(result)
    {
        for idx,Account in result
        {
            AccountId := Account.id
            this.Client.AccountApi.GetAccount(AccountId).Then(ObjBindMethod(this, "_DumpData", "Account Info"), ObjBindMethod(this, "_HandleError", "GetAccount()"))
            this.Client.AccountApi.GetAcademy(AccountId).Then(ObjBindMethod(this, "_DumpData", "Academy"), ObjBindMethod(this, "_HandleError", "GetAcademy()"))
        }
    }
}

test := new TestClass()
test.Run()