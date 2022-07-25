import { Deferred } from './Deferred';
import fetch from 'node-fetch-cjs';

interface DiscoveryResponse {
  resources: { '@id': string; '@type': string }[];
}

interface DataResponse {
  data: string[];
}

export interface NugetVersionOptions {
  prerelease?: boolean;
}

export class NugetVersionLookup {
  /** @type {Map<string, Deferred<string[]>>}*/
  versionMap = new Map<string, Deferred<string[]>>();
  autoCompleteServiceUrl = new Deferred();

  constructor(source?: string) {
    this.init(source);
  }

  reset() {
    this.versionMap.clear();
  }

  async init(source?: string) {
    const discoveryResponse = await fetch(source || 'https://api.nuget.org/v3/index.json', {
      headers: { accept: 'application/json' },
    });
    const { resources } = (await discoveryResponse.json()) as DiscoveryResponse;
    this.autoCompleteServiceUrl.resolve(resources.find((res) => res['@type'] == 'SearchAutocompleteService')!['@id']);
  }

  async version(id: string, version: string, { prerelease }: NugetVersionOptions = {}) {
    const versionsData = await this.versions(id, { prerelease });
    return versionsData.includes(version);
  }

  async latest(id: string, { prerelease }: NugetVersionOptions = {}) {
    const versionsData = await this.versions(id, { prerelease });
    return versionsData[versionsData.length - 1];
  }

  async versions(id: string, { prerelease }: NugetVersionOptions = {}) {
    const mapKey = `${id};pr:${prerelease}`;
    let versionsData = this.versionMap.get(mapKey);
    if (versionsData) {
      return versionsData;
    }
    versionsData = new Deferred();
    this.versionMap.set(mapKey, versionsData);

    // get version data
    {
      const baseUrl = await this.autoCompleteServiceUrl;
      const versionsResponse = await fetch(`${baseUrl}?id=${encodeURIComponent(id)}&prerelease=${prerelease}`);
      const { data } = (await versionsResponse.json()) as DataResponse;
      versionsData.resolve(data);
    }

    return versionsData;
  }
}
