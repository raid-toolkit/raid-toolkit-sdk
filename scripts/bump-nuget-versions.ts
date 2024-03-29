import { execSync } from 'child_process';
import fs from 'fs';
import path from 'path';
import semver from 'semver';
import chalk from 'chalk';
import cliSpinners, { Spinner } from 'cli-spinners';
import { NugetVersionLookup } from './NugetVersionLookup';
import { showToast } from './Toast';

export interface BumpVerionsOptions {
  whatIf?: boolean;
  latest?: boolean;
  wait?: boolean;
  includePrerelease?: boolean;
  log?: boolean;
  source?: string;
}

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

function* getCsProjFiles() {
  const slnFilePath = path.join(__dirname, '../SDK.sln');
  const slnContent = fs.readFileSync(slnFilePath, { encoding: 'utf8' });
  const extractCsProjRegexp =
    /Project\("{[0-9A-F\-]+}"\) = "[\w\.]+", "(?<csproj>[\w\\\.]+\.csproj)", "{[0-9A-F\-]+}"/gm;

  let result = extractCsProjRegexp.exec(slnContent)?.[1];
  while (result) {
    yield result;
    result = extractCsProjRegexp.exec(slnContent)?.[1];
  }
}

const resetPosition = '\x1b[0G';

function writeStatus({ interval, frames }: Spinner) {
  let n = 0;
  const handle = setInterval(() => {
    process.stdout.write(
      chalk.yellowBright`  Waiting for packages to be available ` +
        chalk.greenBright(frames[n++ % frames.length]) +
        resetPosition
    );
  }, interval);
  return () => {
    console.log('');
    clearInterval(handle);
  };
}

async function waitAndRun(opts: BumpVerionsOptions) {
  const clearStatus = writeStatus(cliSpinners.bouncingBar);

  const nugetVersions = new NugetVersionLookup(opts.source);
  while (!(await run({ ...opts, log: false, whatIf: true }))) {
    nugetVersions.reset();
    await delay(30000);
  }
  clearStatus();
  showToast('Updating packages', `New packages were found on ${opts.source}`);
  return run(opts);
}

async function run({ log, whatIf, latest, includePrerelease, source }: BumpVerionsOptions) {
  let doRestore = false;
  let hasErrors = false;
  const nugetVersions = new NugetVersionLookup(source);
  for (const csProjFilePath of getCsProjFiles()) {
    log && console.log('📄 ' + chalk.magentaBright(csProjFilePath));
    const replaceVersionRegexp =
      /([<]PackageReference\s+Include=")(Il2CppToolkit\..+?)("\s+Version=")([\d\.\-\w]+)("\s)/gim;
    const csProjContent = fs.readFileSync(path.resolve(__dirname, '..', csProjFilePath), {
      encoding: 'utf8',
    });

    let write = false;

    const versionMap = new Map();
    for (const [, , pkgName, , version] of csProjContent.matchAll(replaceVersionRegexp)) {
      const currentVersion = semver.parse(version);
      if (!currentVersion) {
        throw new Error(`Could not parse version '${version}'`);
      }
      let newVersionStr;
      if (latest) {
        newVersionStr = await nugetVersions.latest(pkgName, {
          prerelease: includePrerelease,
        });
      } else {
        const newVersion = currentVersion.inc('prepatch', currentVersion.prerelease?.[0] as string);
        const newVersionStrCandidate = `${newVersion.major}.${newVersion.minor}.${newVersion.patch}${
          newVersion.prerelease[0] ? `-${newVersion.prerelease[0]}` : ''
        }`;
        const hasVersion = await nugetVersions.version(pkgName, newVersionStrCandidate, {
          prerelease: includePrerelease,
        });
        if (!hasVersion) {
          hasErrors = true;
          log && console.error(chalk.redBright(`ERROR: Missing package ${pkgName}@${newVersionStrCandidate}.`));
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
        // @ts-ignore
        doRestore |= write |= newVersion !== version;
        log &&
          console.log(
            `  📦 ${chalk.green(pkgName.padEnd(32, ' '))}${chalk.yellow(version.padEnd(13, ' '))} -> ${
              newVersion !== version ? chalk.greenBright(newVersion) : chalk.yellow(newVersion)
            }`
          );
        return [prefix, pkgName, mid, newVersion, end].join('');
      }
    );

    if (!write) continue;

    log && console.log('  📝 ' + chalk.greenBright`Writing file...`);
    if (!whatIf && !hasErrors) {
      fs.writeFileSync(csProjFilePath, csProjContentReplaced, {
        encoding: 'utf8',
      });
    }
  }

  if (hasErrors) {
    return false;
  }

  if (!doRestore) {
    log && console.log(chalk.greenBright('No changes to apply'));
    return false;
  }

  log && console.log(chalk.yellowBright`Installing new dependencies`);
  if (!whatIf) {
    execSync('dotnet restore --no-cache', { stdio: 'inherit' });
  }

  //   log && console.log(chalk.yellowBright`Removing built interop dlls`);
  //   if (!whatIf) {
  //     rimraf.sync("**/raid.interop.dll");
  //   }
  return true;
}

export function bumpNugetVersions(opts: BumpVerionsOptions) {
  if (opts.wait) {
    waitAndRun({ log: true, ...opts });
  } else {
    run({ log: true, ...opts });
  }
}
