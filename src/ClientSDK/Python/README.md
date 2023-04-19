# RaidToolkit Client

This package provides access to the Raid Toolkit API

# Installation

```sh
pip3 install raidtoolkit
```

# Usage

To access APIs from RTK, use the RaidToolkitClient class to connect and read account information.

```py
import asyncio
from raidtoolkit import RaidToolkitClient

async def main():
    client = RaidToolkitClient()
    client.connect()
    try:
        accounts = await client.get_connected_accounts()
        for account in accounts:
            print(f'Account: {account.name}')
            
            viewInfo = await account.get_current_view_info()
            print(f'\tCurrent View: {viewInfo["viewKey"]} ({viewInfo["viewId"]})')
            
            lastBattleResponse = await account.get_last_battle_response()
            print(f'\tLast Battle Response: {lastBattleResponse["turnCount"]} turns')

            heroes = await account.get_heroes()
            nLegendary = len(list(filter(lambda hero: hero["type"]["rarity"] == 'Legendary', heroes)))
            print(f'\tHero count: {len(heroes)} ({nLegendary} legendary)')

    finally:
        # make sure to always close the client, or the python process will continue to run until the client is closed.
        client.close()

if __name__ == "__main__":
    asyncio.run(main())
```

# APIs

## `RaidToolkitClient`

### Properties

#### `StaticDataApi`

Provides access to the raw [`StaticDataApi`](#staticdataapi-1) API.

#### `AccountApi`

> NOTE: All functions in the `AccountApi` are also available on the [`Account`](#account) object returned from `get_*_accounts` APIs on the `RaidToolkitClient` class.

Provides access to the raw [`AccountApi`](#accountapi-1) API.

#### `RealtimeApi`

> NOTE: All functions in the `RealtimeApi` are also available on the [`Account`](#account) object returned from `get_*_accounts` APIs on the `RaidToolkitClient` class.

Provides access to the raw [`RealtimeApi`](#realtimeapi-1) API.

### Methods

#### `get_all_accounts()`

Gets a list of `Account` which RTK has saved data for. Not all accounts may have a game client running.

```py
accounts = client.get_all_accounts()
for account in accounts:
    print(f'Account: {account.name}')
```

#### `get_connected_accounts()`

Gets a list of [`Account`](#account) which RTK has saved data for, and have a game client running.

```py
accounts = client.get_connected_accounts()
for account in accounts:
    print(f'Account: {account.name}')
```

### `StaticDataApi`

#### Methods

##### `get_all_data()`

Gets all static data from RTK.

```py
data = client.StaticDataApi.get_all_data()
print(data)
```

##### `get_localized_strings()`

Gets all localized strings from RTK.

```py
strings = client.StaticDataApi.get_localized_strings()
print(strings["l10n:skill/description?id=57203#static"]) # "Increases the damage inflicted by this Champion equal to the percentage of MAX HP destroyed on the target."
```

##### `get_arena_data()`

Gets all arena data from RTK.

```py
arena_data = client.StaticDataApi.get_arena_data()
print(arena_data)
```

##### `get_artifact_data()`

Gets all artifact data from RTK.

```py
artifact_data = client.StaticDataApi.get_artifact_data()
print(artifact_data)
```

##### `get_hero_data()`

Gets all hero data from RTK.

```py
hero_data = client.StaticDataApi.get_hero_data()
print(hero_data)
```

##### `get_skill_data()`

Gets all skill data from RTK.

```py
skill_data = client.StaticDataApi.get_skill_data()
print(skill_data)
```

##### `get_stage_data()`

Gets all stage data from RTK.

```py
stage_data = client.StaticDataApi.get_stage_data()
print(stage_data)
```

## `Account`

Object which represents a single account in RTK. Account may or may not be connected

### Methods

#### `get_dump()`

Gets a dump of the account in RaidExtractor format.

```py
dump = account.get_dump()
print(dump)
```

#### `get_artifacts()`

Gets a list of artifacts for the account.

```py
artifacts = account.get_artifacts()
for artifact in artifacts:
    print(f'Artifact: {artifact}') # don't actually do this lol
```

#### `get_heroes(snapshot: bool)`

Gets a list of heroes for the account.

```py
strings = client.StaticDataApi.get_localized_strings()
heroes = account.get_heroes()
for hero in heroes:
    print(f'Hero #{hero["id"]}: {strings[hero["type"]["name"]["key"]]}')
```

#### `get_resources()`

Gets a list of resources for the account.

```py
resources = account.get_resources()
print(resources)
```

#### `get_resources()`

Gets a list of resources for the account.

```py
resources = account.get_resources()
print(resources)
```

#### `get_academy()`

Gets the faction guardian information for the account.

```py
academy = account.get_academy()
print(academy)
```

#### `get_last_battle_response()`

Gets the last battle response for the account.

```py
last_battle_response = account.get_last_battle_response()
print(last_battle_response)
```

#### `get_current_view_info()`

Gets the current view info for the account.

```py
current_view_info = account.get_current_view_info()
print(f'\tCurrent View: {current_view_info["viewKey"]} ({current_view_info["viewId"]})')
```

