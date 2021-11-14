import './ws-polyfill';
import { useRaidToolkitApi, IAccountApi } from '@raid-toolkit/webclient';

async function run() {
  const api = useRaidToolkitApi(IAccountApi);
  let lastUpdated;
  while (true) {
    console.log(`getAccount: ${new Date().toISOString()}`);
    const firstAccount = (await api.getAccounts())[0];
    console.log(`getAccountDump: ${new Date().toISOString()}`);
    await api.getAccountDump(firstAccount.id);
    if (firstAccount.lastUpdated !== lastUpdated) {
      console.log(firstAccount);
      lastUpdated = firstAccount.lastUpdated;
    }
  }
}

run();
