# RaidToolkit WebClient

This package provides access to the Raid Toolkit API from web contexts.

## Installation

### Yarn

```sh
yarn add @raid-toolkit/webclient
```

### NPM

```sh
npm install @raid-toolkit/webclient
```

## Usage

To access APIs from RTK, simply call `useRaidToolkitApi` with the API definition you wish to access, and then call whatever APIs you'd like.

```ts
import { useRaidToolkitApi, IAccountApi } from '@raid-toolkit/webclient';

async function loadAccount() {
    const accountApi = useRaidToolkitApi(IAccountApi);
    const accounts = await accountApi.getAccounts();
    for (const account of accounts) {
        const accountDump = await accountApi.getAccountDump(account.id);
        console.log({account, accountDump});
    }
}

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
| `getArena(accountId)` | Get arena data for a given `accountId` |
| `getAllResources(accountId)` | Get all resources for a given `accountId` |
