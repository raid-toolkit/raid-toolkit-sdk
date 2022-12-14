[![Nuget](https://img.shields.io/nuget/v/raid.client)](https://www.nuget.org/packages/Raid.Client)
[![npm (scoped)](https://img.shields.io/npm/v/@raid-toolkit/webclient)](https://www.npmjs.com/package/@raid-toolkit/webclient)
[![PyPI](https://img.shields.io/pypi/v/raidtoolkit)](https://pypi.org/project/raidtoolkit)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/raid-toolkit/raid-toolkit-sdk)](https://github.com/raid-toolkit/raid-toolkit-sdk/releases/latest)
![GitHub](https://img.shields.io/github/license/raid-toolkit/raid-toolkit-sdk)
[![GitHub issues](https://img.shields.io/github/issues/raid-toolkit/raid-toolkit-sdk)](https://github.com/raid-toolkit/raid-toolkit-sdk/issues)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/raid-toolkit/raid-toolkit-sdk/.NET%20Core%20Desktop)

# Raid Toolkit SDK

The Raid Toolkit ("RTK") SDK is a set of tools and libraries that allow developers to write tools and scripts for **Raid: Shadow Legends** without needing to worry about game updates, memory addressing, or any of that (honestly pretty fun 😏) stuff.

One of the key features of this platform is that it will automatically update itself when the game updates, so users do not need to wait for the developer to release a new version unless something substantial changed in the game code. This is accomplished by dynamically generating a .NET assembly used to read the game objects on the fly.

## Mission

The mission of RTK is twofold:

> ### 🔐 Provide an open-source toolchain that users can trust which handles all of the native aspects of reading from the game data
>
> Reducing the fragmentation of having to install different tools from different developers' means users don't have to worry as much about which tools to trust, and instead focus on tools built atop an already trusted application.

> ### 👩‍💻 Provide a set of well-shaped APIs and data contracts for developers to create new tools
>
> Developing applications which read from process memory is difficult and requires constant maintenance. This SDK provides a solution for those developers so they can focus on making great tools, and not pulling out their hair.

## Installation

In order to take advantage of RTK-powered applications, you need to install the latest RTK Service application. This application will run in the background on your system and watch for Raid processes, and when it finds them it will keep scanning and updating your account data, storing it on your computer. This means your data will always be up to date, **_and it will be available even when the game isn't running!_**

1. Head on over to [Releases](https://github.com/raid-toolkit/raid-toolkit-sdk/releases) and download the `RaidToolkitSetup.exe` artifact from the most recent release.
2. Run `RaidToolkitSetup.exe` and it will guide you through the installation process.

### Building from source

All RTK builds are built directly in our CI and uploaded as a github release, but if you'd feel more comfortable building it locally you can clone the repo and build it yourself. Simply follow the details over at [Developers](DEVELOPERS.md).

## Developers

Developer? Check out these additional links:

- [Developers](./DEVELOPERS.md)
- [Contributing](./CONTRIBUTING.md)
- [Nitty Gritty](./NITTYGRITTY.md)

## Acknolwedgements

This software uses components based on Il2CppDumper by Prefare, as well as leveraging several other great open-source libraries.

Please check [Third Party Notice](./ThirdPartyNotice.txt) for the full list and go support their authors
