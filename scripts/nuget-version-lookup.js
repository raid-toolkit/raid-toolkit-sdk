class Deferred {
  constructor() {
    this.promise = new Promise((resolve, reject) => {
      this.resolve = resolve;
      this.reject = reject;
    });
  }
  then(onfulfilled, onrejected) {
    return this.promise.then(onfulfilled, onrejected);
  }
  catch(onrejected) {
    return this.promise.catch(onrejected);
  }
}

class NugetVersionLookup {
  /** @type {Map<string, Deferred<string[]>>}*/
  versionMap = new Map();
  autoCompleteServiceUrl = new Deferred();

  constructor() {
    this.init();
  }

  async init() {
    const discoveryResponse = await fetch(
      "https://api.nuget.org/v3/index.json",
      { headers: { accept: "application/json" } }
    );
    const { resources } = await discoveryResponse.json();
    this.autoCompleteServiceUrl = resources.find(
      (res) => res["@type"] == "SearchAutocompleteService"
    )["@id"];
  }

  async version(id, version) {
    const versionsData = await this.versions(id);
    return versionsData.includes(version);
  }

  async latest(id) {
    const versionsData = await this.versions(id);
    return versionsData[versionsData.length - 1];
  }

  async versions(id, { prerelease = false }) {
    /**
     * @type {Deferred<string[]>|undefined}
     **/
    let versionsData = this.versionMap.get(id);
    if (versionsData) {
      return versionsData;
    }
    versionsData = new Deferred();
    this.versionMap.set(id, versionsData);

    // get version data
    {
      const baseUrl = await this.autoCompleteServiceUrl;
      const versionsResponse = await fetch(
        `${baseUrl}?id=${encodeURIComponent(id)}&prerelease=${prerelease}`
      );
      const { data } = await versionsResponse.json();
      versionsData.resolve(data);
    }

    return versionsData;
  }
}
