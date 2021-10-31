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

## 👷‍♀️ Coming soon
