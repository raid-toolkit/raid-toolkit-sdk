# RaidToolkit Client

This package provides access to the Raid Toolkit API

## Installation

```sh
pip3 install raidtoolkit
```

## Usage

To access APIs from RTK, simply call `useRaidToolkitApi` with the API definition you wish to access, and then call whatever APIs you'd like.

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

## APIs

### AccountApi

| API | Description |
|-----|-------------|
| `getAccounts` | Fetch a list of all user accounts. |
| `getAccountDump(accountId)` | Get an account dump for a given `accountId`, in RaidExtractor format. |
| `getArtifacts(accountId)` | Get all artifacts for a given `accountId` |
| `getArtifactById(accountId, artifactId)` | Get an artifact by id |
| `getHeroes(accountId)` | Get all heroes for a given `accountId` |
| `getHeroById(accountId, heroId)` | Get a hero by id |
| `getAllResources(accountId)` | Get all resources for a given `accountId` |

