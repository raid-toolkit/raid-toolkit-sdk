# Raid.Toolkit.Build.Tasks

Provides build tasks for Raid Toolkit extensions, compatible with both `dotnet` and `msbuild` build systems (including Visual Studio).

## Usage

Install the package into your project:

```bash
dotnet add package Raid.Toolkit.Build.Tasks
```

### Create a manifest file

```jsonc
{
  "id": "TestExtension",
  "type": "TestExtension.Extension",
  "asm": "TestExtension.dll",
  "displayName": "TestExtension",
  "description": "Extracts account data",
  "requireVersion": "3.0.0",
  "compatibleVersion": "3.0.0",
  "codegen": {
    "types": [ // types to generate (regex)
      "^Client\\.Model\\.AppModel$",
    ]
  }
}
```

Add the following to your projects `.csproj` file:

```xml
<ItemGroup>
  <RTKExtensionManifest Include="manifest.json" />
</ItemGroup>
```

After building, the extension will be bundled as an installable `.rtkx` file in the build output directory.
