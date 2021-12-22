'use strict';

import { AccountInfo } from '@raid-toolkit/webclient';

const { IAccountApi, IStaticDataApi, IRuntimeApi, useRaidToolkitApi } = require('@raid-toolkit/webclient');
require('./ws-polyfill');

(async function run() {
  const accountApi = useRaidToolkitApi(IAccountApi);
  const staticApi = useRaidToolkitApi(IStaticDataApi);
  const runtimeApi = useRaidToolkitApi(IRuntimeApi);
  const account = (await accountApi.getAccounts())[0];

  Object.assign(globalThis, { accountApi, staticApi, runtimeApi, account });
})();

export { IAccountApi, IStaticDataApi, IRuntimeApi };
declare global {
  const accountApi: typeof IAccountApi;
  const staticApi: typeof IStaticDataApi;
  const runtimeApi: typeof IRuntimeApi;
  const account: AccountInfo;
}
