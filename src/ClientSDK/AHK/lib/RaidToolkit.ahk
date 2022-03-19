#Include, <RemoteApiClient>
#Include, <AccountApi>

class RaidToolkitClient extends RemoteApiClient
{
    AccountApi := 
    __New(Socket = "ws://127.0.0.1:9090")
    {
        base.__New(Socket)
        this.AccountApi := new AccountApi(this)
    }
}