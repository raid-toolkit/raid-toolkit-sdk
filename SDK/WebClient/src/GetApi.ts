import { useApi, useRouter } from '@remote-ioc/runtime';
import { RaidToolkitClient } from './Client/RTKClient';
import { RTKClientRouter } from './Client/RTKClientRouter';
import * as APIs from './APIDefinitions';
import { ValueOf } from './Types';

let isInitialized = false;
function ensureInit() {
  if (isInitialized) return;
  const client = new RaidToolkitClient();
  useRouter(RTKClientRouter, client);
  isInitialized = true;
}

export function useRaidToolkitApi(definition: ValueOf<typeof APIs>) {
  ensureInit();
  return useApi(definition);
}
