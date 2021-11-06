import { useApi, useRouter } from '@remote-ioc/runtime';
import { WebSocketClientRouter } from '@remote-ioc/ws-router';
import { RaidToolkitClient } from './Client/RTKClient';
import { RTKClientRouter } from './Client/RTKClientRouter';
import * as APIs from './APIDefinitions';
import { ValueOf } from './Types';

let isInitialized = false;
function ensureInit(proxy: boolean) {
  if (isInitialized) return;
  if (proxy) {
    const client = new RaidToolkitClient();
    useRouter(RTKClientRouter, client);
  } else {
    useRouter(WebSocketClientRouter, new WebSocket('ws://localhost:9090'));
  }
  isInitialized = true;
}

export function useRaidToolkitApi(definition: ValueOf<typeof APIs>, proxy: boolean = false) {
  ensureInit(proxy);
  return useApi(definition);
}
