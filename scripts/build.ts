import { spawn } from 'child-process-promise';
import path from 'path';
import { BuildOptions } from './options';

let nLog = 0;
export function build(opts: BuildOptions) {
  const cmdArgs = [
    opts.project,
    // '-verbosity:d',
    // `-fileLogger${1 + (nLog++ % 9)}`,
    '-m:1',
    `-p:Configuration=${opts.flavor}`,
    `-p:Platform=${opts.platform}`,
    opts.targetFramework && `-p:TargetFramework=${opts.targetFramework}`,
    `-t:${opts.targets.join(';')}`,
    '-p:IncludeSymbols=true',
    '-p:SymbolPackageFormat=snupkg',
    `-p:PackageOutputPath=${path.format(path.posix.parse(opts.packageDir))}`,
    `-p:PublishDir=${path.format(path.posix.parse(opts.publishDir))}`,
  ].filter(Boolean) as string[];
  return spawn(`${process.env.VSInstallDir}/MSBuild/Current/Bin/amd64/MSBuild.exe`, cmdArgs, {
    stdio: 'inherit',
    cwd: opts.basePath,
  });
}
