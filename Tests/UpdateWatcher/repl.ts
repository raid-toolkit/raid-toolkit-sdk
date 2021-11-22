'use strict';

import { AccountInfo } from '@raid-toolkit/webclient';

const { IAccountApi, useRaidToolkitApi } = require('@raid-toolkit/webclient');
require('./ws-polyfill');

(async function run() {
  const api = useRaidToolkitApi(IAccountApi);
  const account = (await api.getAccounts())[0];

  Object.assign(globalThis, { api, account });
})();

export { IAccountApi };
declare global {
  const api: typeof IAccountApi;
  const account: AccountInfo;
}
