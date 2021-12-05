import { IAccountApi, useRaidToolkitApi } from '@raid-toolkit/webclient';
import { diff } from 'jsondiffpatch';
import { inspect } from 'util';
import './ws-polyfill';

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

async function run2() {
  const api = useRaidToolkitApi(IAccountApi);
  const account = (await api.getAccounts())[0];
  let lastDump = await api.getAccountDump(account.id);
  api.on('updated', async (accountId) => {
    console.log({ accountId });
    let dump = await api.getAccountDump(account.id);
    const patch = diff(lastDump, dump);
    lastDump = dump;
    if (patch) console.log(inspect(patch, false, 5, true));
  });
}

run2();
