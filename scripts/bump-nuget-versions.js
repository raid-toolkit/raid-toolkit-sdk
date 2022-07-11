import { execSync } from "child_process";
import fs from "fs";
import path from "path";
import semver from "semver";
import rimraf from "rimraf";
import chalk from "chalk";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const whatIf =
  process.argv.includes("--what-if") || process.argv.includes("-n");

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

for (const csProjFilePath of getCsProjFiles()) {
  console.log("📄 " + chalk.cyanBright(csProjFilePath));
  const replaceVersionRegexp =
    /([<]PackageReference\s+Include=")(Il2CppToolkit\..+?)("\s+Version=")([\d\.\-\w]+)(".*\/[>])/gim;
  const csProjContent = fs.readFileSync(
    path.resolve(__dirname, "..", csProjFilePath),
    {
      encoding: "utf8",
    }
  );

  let write = false;
  const csProjContentReplaced = csProjContent.replace(
    replaceVersionRegexp,
    (_, ...[prefix, pkgName, mid, version, end]) => {
      write = true;
      const currentVersion = semver.parse(version);
      const newVersion = currentVersion.inc(
        "prepatch",
        currentVersion.prerelease[0]
      );
      const newVersionStr = `${newVersion.major}.${newVersion.minor}.${
        newVersion.patch
      }${newVersion.prerelease[0] ? `-${newVersion.prerelease[0]}` : ""}`;
      console.log(
        `  📦 ${chalk.green(pkgName.padEnd(32, " "))}${chalk.yellow(
          version.padEnd(13, " ")
        )} -> ${chalk.greenBright(newVersionStr)}`
      );
      return [prefix, pkgName, mid, newVersionStr, end].join("");
    }
  );

  if (!write) continue;

  console.log("  📝 " + chalk.greenBright("Writing file..."));
  if (!whatIf) {
    fs.writeFileSync(csProjFilePath, csProjContentReplaced, {
      encoding: "utf8",
    });
  }
}

console.log("Installing new dependencies");
if (!whatIf) {
  execSync("dotnet restore --no-cache", { stdio: "inherit" });
}

console.log("Removing built interop dlls");
if (!whatIf) {
  rimraf.sync("**/raid.interop.dll");
}
