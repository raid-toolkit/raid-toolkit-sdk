import caporal from 'caporal';
import path from 'path';
import { bumpNugetVersions, BumpVerionsOptions } from './bump-nuget-versions';
import { build } from './build';
import { buildOptions, BuildOptions } from './options';

const nugetSources = {
  nuget: 'https://api.nuget.org/v3/index.json',
  local: 'http://localhost:8090/v3/index.json',
};

const cli = caporal.version('1.0.0');

cli
  .command('bump-nuget', 'Bump nuget versions')
  .option('-n, --what-if', 'Only print what would be run, without writing any changes')
  .option('-l, --latest', 'Choose the latest version available')
  .option('-w, --wait', 'Wait until versions become available')
  .option('-p, --include-prerelease', 'Include prerelease builds')
  .option('-s, --source <source>', 'Nuget source', Object.keys(nugetSources))
  .action(bumpVersions);

cli
  .command('publish', 'Build deployable packages')
  .option('-o, --only', 'Publish only')
  .option('-f, --flavor <flavor>', 'Build flavor', ['Debug', 'Release'], 'Debug')
  .option('-p, --platform <platform>', 'Platform', ['x64'], 'x64')
  .action(publishBuild);

cli.parse(process.argv);

function bumpVersions(_args: any, opts: BumpVerionsOptions, _logger: any) {
  opts.source = nugetSources[(opts.source || 'nuget') as keyof typeof nugetSources] ?? nugetSources.nuget;
  return bumpNugetVersions(opts);
}

async function publishBuild(_args: any, opts: Partial<BuildOptions>, _logger: any) {
  if (!opts.only) {
    await build(
      buildOptions({
        targets: ['Restore', 'Build'],
        ...opts,
      })
    );
  }
  await build(
    buildOptions({
      project: path.resolve(__dirname, '../src/Application/Raid.Toolkit/Raid.Toolkit.csproj'),
      targetFramework: 'net6.0-windows10.0.19041.0',
      targets: ['Publish'],
      ...opts,
    })
  );
}
