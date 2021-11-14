import './ws-polyfill';
import { useRaidToolkitApi, IAccountApi } from '@raid-toolkit/webclient';

async function run() {
  const api = useRaidToolkitApi(IAccountApi);
  let lastUpdated;
  while (true) {
    const firstAccount = (await api.getAccounts())[0];
    if (firstAccount.lastUpdated !== lastUpdated) {
      console.log(firstAccount);
      lastUpdated = firstAccount.lastUpdated;
    }
  }
}

run();
