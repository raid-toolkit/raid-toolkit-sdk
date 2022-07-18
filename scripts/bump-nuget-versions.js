import { execSync } from "child_process";
import fs from "fs";
import path from "path";
import semver from "semver";
import rimraf from "rimraf";
import chalk from "chalk";
import { fileURLToPath } from "url";
import fetch from "node-fetch";
import cliSpinners from "cli-spinners";
const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

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

  reset() {
    this.versionMap.clear();
  }

  async init() {
    const discoveryResponse = await fetch(
      "https://api.nuget.org/v3/index.json",
      { headers: { accept: "application/json" } }
    );
    const { resources } = await discoveryResponse.json();
    this.autoCompleteServiceUrl.resolve(
      resources.find((res) => res["@type"] == "SearchAutocompleteService")[
        "@id"
      ]
    );
  }

  async version(id, version, { prerelease } = {}) {
    const versionsData = await this.versions(id, { prerelease });
    return versionsData.includes(version);
  }

  async latest(id, { prerelease } = {}) {
    const versionsData = await this.versions(id, { prerelease });
    return versionsData[versionsData.length - 1];
  }

  async versions(id, { prerelease } = {}) {
    const mapKey = `${id};pr:${prerelease}`;
    /**
     * @type {Deferred<string[]>|undefined}
     **/
    let versionsData = this.versionMap.get(mapKey);
    if (versionsData) {
      return versionsData;
    }
    versionsData = new Deferred();
    this.versionMap.set(mapKey, versionsData);

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

const nugetVersions = new NugetVersionLookup();

function* getCsProjFiles() {
  const slnFilePath = path.join(__dirname, "../SDK.sln");
  const slnContent = fs.readFileSync(slnFilePath, { encoding: "utf8" });
  const extractCsProjRegexp =
    /Project\("{[0-9A-F\-]+}"\) = "[\w\.]+", "(?<csproj>[\w\\\.]+\.csproj)", "{[0-9A-F\-]+}"/gm;

  let result = extractCsProjRegexp.exec(slnContent)?.[1];
  while (result) {
    yield result;
    result = extractCsProjRegexp.exec(slnContent)?.[1];
  }
}

const resetPosition = "\x1b[0G";

function writeStatus({ interval, frames }) {
  let n = 0;
  const handle = setInterval(() => {
    process.stdout.write(
      chalk.yellowBright`  Waiting for packages to be available ` +
        chalk.greenBright(frames[n++ % frames.length]) +
        resetPosition
    );
  }, interval);
  return () => {
    console.log("");
    clearInterval(handle);
  };
}

async function waitAndRun(opts) {
  const clearStatus = writeStatus(cliSpinners.bouncingBar);
  while (!(await run({ ...opts, log: false, whatIf: true }))) {
    nugetVersions.reset();
    await delay(30000);
  }
  clearStatus();
  return run(opts);
}

async function run({ log, whatIf, latest, includePrerelease }) {
  let doRestore = false;
  let hasErrors = false;
  for (const csProjFilePath of getCsProjFiles()) {
    log && console.log("📄 " + chalk.magentaBright(csProjFilePath));
    const replaceVersionRegexp =
      /([<]PackageReference\s+Include=")(Il2CppToolkit\..+?)("\s+Version=")([\d\.\-\w]+)(".*\/[>])/gim;
    const csProjContent = fs.readFileSync(
      path.resolve(__dirname, "..", csProjFilePath),
      {
        encoding: "utf8",
      }
    );

    let write = false;

    const versionMap = new Map();
    for (const [, , pkgName, , version] of csProjContent.matchAll(
      replaceVersionRegexp
    )) {
      const currentVersion = semver.parse(version);
      let newVersionStr;
      if (latest) {
        newVersionStr = await nugetVersions.latest(pkgName, {
          prerelease: includePrerelease,
        });
      } else {
        const newVersion = currentVersion.inc(
          "prepatch",
          currentVersion.prerelease[0]
        );
        const newVersionStrCandidate = `${newVersion.major}.${
          newVersion.minor
        }.${newVersion.patch}${
          newVersion.prerelease[0] ? `-${newVersion.prerelease[0]}` : ""
        }`;
        const hasVersion = await nugetVersions.version(
          pkgName,
          newVersionStrCandidate,
          { prerelease: includePrerelease }
        );
        if (!hasVersion) {
          hasErrors = true;
          log &&
            console.error(
              chalk.redBright(
                `ERROR: Missing package ${pkgName}@${newVersionStrCandidate}.`
              )
            );
          continue;
        }

        newVersionStr = newVersionStrCandidate;
      }
      versionMap.set(pkgName, newVersionStr);
    }

    const csProjContentReplaced = csProjContent.replace(
      replaceVersionRegexp,
      (line, ...[prefix, pkgName, mid, version, end]) => {
        const newVersion = versionMap.get(pkgName);
        if (!newVersion) {
          return line;
        }
        doRestore |= write |= newVersion !== version;
        log &&
          console.log(
            `  📦 ${chalk.green(pkgName.padEnd(32, " "))}${chalk.yellow(
              version.padEnd(13, " ")
            )} -> ${
              newVersion !== version
                ? chalk.greenBright(newVersion)
                : chalk.yellow(newVersion)
            }`
          );
        return [prefix, pkgName, mid, newVersion, end].join("");
      }
    );

    if (!write) continue;

    log && console.log("  📝 " + chalk.greenBright`Writing file...`);
    if (!whatIf && !hasErrors) {
      fs.writeFileSync(csProjFilePath, csProjContentReplaced, {
        encoding: "utf8",
      });
    }
  }

  if (hasErrors) {
    return false;
  }

  if (!doRestore) {
    log && console.log(chalk.greenBright("No changes to apply"));
    return false;
  }

  log && console.log(chalk.yellowBright`Installing new dependencies`);
  if (!whatIf) {
    execSync("dotnet restore --no-cache", { stdio: "inherit" });
  }

  log && console.log(chalk.yellowBright`Removing built interop dlls`);
  if (!whatIf) {
    rimraf.sync("**/raid.interop.dll");
  }
  return true;
}

function main() {
  const whatIf =
    process.argv.includes("--what-if") || process.argv.includes("-n");
  const latest =
    process.argv.includes("--latest") || process.argv.includes("-l");
  const wait = process.argv.includes("--wait") || process.argv.includes("-w");
  const includePrerelease =
    process.argv.includes("--prerelease") ||
    process.argv.includes("--pre-release") ||
    process.argv.includes("-p");

  if (wait) {
    waitAndRun({ log: true, whatIf, latest, includePrerelease });
  } else {
    run({ log: true, whatIf, latest, includePrerelease });
  }
}
main();
