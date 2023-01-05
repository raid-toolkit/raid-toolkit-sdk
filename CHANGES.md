# Release Notes

## 2.2.x
* #114 - removed usage of get_ValueCap in skills data which for some reason seems to be missing from actual gasm dll.
* #109, #112 - Updated window APIs to allow for both WinForms and WinUI windows to be managed by RTK and provide lighter weight usage patterns.
* #111 - Handle extension re-installation
* #110 - Update logo graphics


**Full Changelog**: https://github.com/raid-toolkit/raid-toolkit-sdk/compare/1919fd4852dc060d671eda83a49cacd6c2ce29ba...v2.2.4.61376

## 2.1.x
* #107 - Changed account resources to return raw value rather than prematurely rounding. This was causing issues for some tools which use things like # of keys to determine whether there are enough resources.

**Full Changelog**: https://github.com/raid-toolkit/raid-toolkit-sdk/compare/v2.1.1.57219...v2.2.1.42006

## 2.0.x
* Introduced standalone installer which will install required .net dependencies and check for compatibility issues
* Adopted WinUI as default UI provider (old winforms UI can be used by launching with the `--render-engine WinForms` argument)
