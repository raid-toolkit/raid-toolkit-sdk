import './ws-polyfill';
import { useRaidToolkitApi, IAccountApi } from '@raid-toolkit/webclient';
import { diff } from 'jsondiffpatch';
import { inspect } from 'util';

async function run() {
  const api = useRaidToolkitApi(IAccountApi);
  const account = (await api.getAccounts())[0];
  let lastDump = await api.getAccountDump(account.id);
  while (true) {
    let dump = await api.getAccountDump(account.id);
    const patch = diff(lastDump, dump);
    lastDump = dump;
    if (!patch) continue;
    console.log(inspect(patch, false, 5, true));
  }
}

run();
