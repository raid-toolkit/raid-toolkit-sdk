'use strict';

import { AccountInfo } from '@raid-toolkit/webclient';

const { IAccountApi, IStaticDataApi, useRaidToolkitApi } = require('@raid-toolkit/webclient');
require('./ws-polyfill');

(async function run() {
  const accountApi = useRaidToolkitApi(IAccountApi);
  const staticApi = useRaidToolkitApi(IStaticDataApi);
  const account = (await accountApi.getAccounts())[0];

  Object.assign(globalThis, { accountApi, staticApi, account });
})();

export { IAccountApi };
declare global {
  const accountApi: typeof IAccountApi;
  const staticApi: typeof IStaticDataApi;
  const account: AccountInfo;
}
