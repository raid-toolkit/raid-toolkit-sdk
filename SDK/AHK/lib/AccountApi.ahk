class AccountApi
{
    Client := 
    __New(Client)
    {
        this.Client := Client
    }

    GetAccounts()
    {
        return this.Client._Call("account-api", "getAccounts")
    }

    GetAccount(AccountId)
    {
        return this.Client._Call("account-api", "accountInfo", [AccountId])
    }

    GetAccountDump(AccountId)
    {
        return this.Client._Call("account-api", "getAccountDump", [AccountId])
    }

    GetAllResources(AccountId)
    {
        return this.Client._Call("account-api", "getAllResources", [AccountId])
    }

    GetArtifacts(AccountId)
    {
        return this.Client._Call("account-api", "getArtifacts", [AccountId])
    }

    GetArtifactById(AccountId, ArtifactId)
    {
        return this.Client._Call("account-api", "getArtifactById", [AccountId, ArtifactId])
    }

    GetHeroes(AccountId)
    {
        return this.Client._Call("account-api", "getHeroes", [AccountId])
    }

    GetHeroById(AccountId, HeroId, Snapshot = false)
    {
        return this.Client._Call("account-api", "getHeroById", [AccountId, HeroId, Snapshot])
    }

    GetArena(AccountId)
    {
        return this.Client._Call("account-api", "getArena", [AccountId])
    }

    GetAcademy(AccountId)
    {
        return this.Client._Call("account-api", "getAcademy", [AccountId])
    }
}