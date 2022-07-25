import caporal from 'caporal';
import { bumpNugetVersions, BumpVerionsOptions } from './bump-nuget-versions';

const cli = caporal.version('1.0.0');

cli
  .command('bump-nuget', 'Bump nuget versions')
  .option('-n, --what-if', 'Only print what would be run, without writing any changes')
  .option('-l, --latest', 'Choose the latest version available')
  .option('-w, --wait', 'Wait until versions become available')
  .option('-p, --include-prerelease', 'Include prerelease builds')
  .action(bumpVersions);

cli.parse(process.argv);

function bumpVersions(_args: any, opts: BumpVerionsOptions, _logger: any) {
  return bumpNugetVersions(opts);
}
