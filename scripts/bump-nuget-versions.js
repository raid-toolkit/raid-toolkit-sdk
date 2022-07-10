const fs = require("fs");
const path = require("path");
const semver = require("semver");

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
  console.log(csProjFilePath);
  const replaceVersionRegexp =
    /([<]PackageReference\s+Include=")(Il2CppToolkit\..+?)("\s+Version=")([\d\.\-\w]+)(".*\/[>])/gim;
  const csProjContent = fs.readFileSync(
    path.resolve(__dirname, "..", csProjFilePath),
    {
      encoding: "utf8",
    }
  );

  const csProjContentReplaced = csProjContent.replace(
    replaceVersionRegexp,
    (_, ...[prefix, pkgName, mid, version, end]) => {
      const currentVersion = semver.parse(version);
      const newVersion = currentVersion.inc(
        "prepatch",
        currentVersion.prerelease[0]
      );
      const newVersionStr = `${newVersion.major}.${newVersion.minor}.${
        newVersion.patch
      }${newVersion.prerelease[0] ? `-${newVersion.prerelease[0]}` : ""}`;
      return [prefix, pkgName, mid, newVersionStr, end].join("");
    }
  );
  fs.writeFileSync(csProjFilePath, csProjContentReplaced, { encoding: "utf8" });
}
