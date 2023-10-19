# Release Notes

## 2.8.x

* Hero equipped artifacts will now update when removed, and will not show as equipped on multiple heroes.

## 2.7.x

* Stabilization and bugfixes

## 2.6.x

* Stabilization and bugfixes

## 2.5.x

* Added VSIX extension template to SDK build
* Updated to Il2CppToolkit 2.0.115-alpha, which adds "(Hooked)" to the window title for hooked processes

## 2.4.x

* #117 - Add account management UX for exporting/importing account information

## 2.3.x

* #116 - Add missing fields:

    > `Raid.Toolkit.DataModel.Hero`:
    >
    > * `AwakenRank` (`awakenRank`)
    > * `Blessing` (`blessing`)
    > * `FreeBlessingResetUsed` (`blessingResetUsed`)
    >
    > `Raid.Toolkit.DataModel.HeroType`:
    > * `ShortName`
    >
    > `Raid.Toolkit.DataModel.Artifact`:
    > * `AscendLevel` (`ascendLevel`)
    > * `AscendBonus` (`ascendBonus`)
    >

* #115 - Settings were not loaded if `--no-webservice` specified, which prevented background services from running in debug process
* `--debug` switch will now use an alternative gold icon in the taskbar to differentiate from other normal RTK processes.

## 2.2.x

* #114 - removed usage of get_ValueCap in skills data which for some reason seems to be missing from actual gasm dll.
* #109, #112 - Updated window APIs to allow for both WinForms and WinUI windows to be managed by RTK and provide lighter weight usage patterns.
* #111 - Handle extension re-installation
* #110 - Update logo graphics

## 2.1.x

* #107 - Changed account resources to return raw value rather than prematurely rounding. This was causing issues for some tools which use things like # of keys to determine whether there are enough resources.

## 2.0.x

* Introduced standalone installer which will install required .net dependencies and check for compatibility issues
* Adopted WinUI as default UI provider (old winforms UI can be used by launching with the `--render-engine WinForms` argument)
