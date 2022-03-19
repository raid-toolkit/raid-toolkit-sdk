"use strict";

import type { AccountInfo } from "@raid-toolkit/webclient";
const {
  IAccountApi,
  IStaticDataApi,
  IRealtimeApi,
  useRaidToolkitApi,
} = require("@raid-toolkit/webclient");
import chalk from "chalk";

const accent = chalk.rgb(235, 97, 52).bold;

(async function run() {
  await import("./ws-polyfill");
  const accountApi = useRaidToolkitApi(IAccountApi);
  const staticApi = useRaidToolkitApi(IStaticDataApi);
  const runtimeApi = useRaidToolkitApi(IRealtimeApi);
  const account = (await accountApi.getAccounts())[0];

  const replExports = { accountApi, staticApi, runtimeApi, account };
  console.log(`Exported APIs:`);
  for (const [key, value] of Object.entries(replExports)) {
    console.log(accent(`🧪 ${key}`));
  }

  Object.assign(globalThis, replExports);
})();

export { IAccountApi, IStaticDataApi, IRealtimeApi };
declare global {
  const accountApi: typeof IAccountApi;
  const staticApi: typeof IStaticDataApi;
  const runtimeApi: typeof IRealtimeApi;
  const account: AccountInfo;
}
