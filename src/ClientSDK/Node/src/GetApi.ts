import { useApi, useRouter } from '@remote-ioc/runtime';
import { WebSocketClientRouter } from '@remote-ioc/ws-router';
import * as APIs from './APIDefinitions';
import { RaidToolkitClient } from './Client/RTKClient';
import { RTKClientRouter } from './Client/RTKClientRouter';
import { ValueOf } from './Types';

let isInitialized = false;
function ensureInit(proxy: boolean) {
  if (isInitialized) return;
  if (proxy) {
    const client = new RaidToolkitClient();
    useRouter(RTKClientRouter, client);
  } else {
    useRouter(WebSocketClientRouter, new WebSocket('ws://127.0.0.1:9090'));
  }
  isInitialized = true;
}

export function useRaidToolkitApi(definition: typeof APIs.IAccountApi, proxy?: boolean): APIs.IAccountApi;
export function useRaidToolkitApi(definition: typeof APIs.IStaticDataApi, proxy?: boolean): APIs.IStaticDataApi;
export function useRaidToolkitApi(definition: typeof APIs.IRealtimeApi, proxy?: boolean): APIs.IRealtimeApi;
export function useRaidToolkitApi(definition: ValueOf<typeof APIs>, proxy: boolean = false) {
  ensureInit(proxy);
  return useApi(definition);
}
