import fs from 'fs';
import { spawnSync } from 'child_process';

const extractorPath = require.resolve('../../../extractor/bin/Debug/net5.0-windows/win-x64/Raid.Extractor.exe');

async function run() {
  if (!fs.existsSync('./temp')) {
    fs.mkdirSync('./temp', { recursive: true });
  }
  let n = 0;
  while (true) {
    console.log(`${new Date().toISOString()}`);
    spawnSync(extractorPath, ['-g', '-o', `./temp/dump.${n++}.json`]);
  }
}

run();
