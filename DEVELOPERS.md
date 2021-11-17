# RTK Development

## Building RTK-powered tools

In order to build a tool using RTK, simply follow the instructions below for your language.

### .NET 5.0

.NET applications must run on the same computer as the RTK Service application, and will communicate with it over the local loopback interface (`wss://localhost:9090`).

1. Install the latest Raid.Client package from nuget:

   ```sh
   dotnet add package Raid.Client
   ```

2. Use the `Raid.Client.RaidToolkitClient` class to connect to the RTK process:

   ```cs
   RaidToolkitClient client = new();
   try
   {
       // connect to the RTK process
       client.Connect();
       // fetch the known user account information
       var accounts = await client.AccountApi.GetAccounts();
       // dump the first account in RaidExtractor format
       AccountDump dump = await client.AccountApi.GetAccountDump(accounts[0].Id);
       // ...
   }
   catch (Exception ex)
   {
       Console.WriteLine($"An error occurred: {ex.Message}");
   }
   finally
   {
       // make sure to disconnect when you're done!
       client.Disconnect();
   }
   ```

### TypeScript/JavaScript

1. Install `Raid.Client` nuget package:

    **NPM:**

    ```sh
    npm install @raid-toolkit/webclient
    ```

    **YarnV1:**

    ```sh
    yarn add @raid-toolkit/webclient
    ```

2. Use the `useRaidToolkitApi` method to construct the API you wish to use:

    ```ts
    import { useRaidToolkitApi, IAccountApi } from '@raid-toolkit/webclient';

    const api = useRaidToolkitApi(IAccountApi);
    const account = (await api.getAccounts())[0];
    let accountDump = await api.getAccountDump(account.id);
    ```

### Python3

1. Install the `raidtoolkit` package:

    ```sh
    pip3 install raidtoolkit
    ```

2. Use the `RaidToolkitClient` class to connect to the RTK process:

    ```py
    import asyncio
    from raidtoolkit import RaidToolkitClient

    async def main():
        client = RaidToolkitClient()
        client.connect()
        accounts = await client.AccountApi.get_accounts()
        print(accounts)
        account = await client.AccountApi.get_account_dump(accounts[0]["id"])
        print(account)
        client.close()

    if __name__ == "__main__":
        asyncio.run(main())
    ```
