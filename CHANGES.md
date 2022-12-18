# Release Notes

## 2.1.x
* #108 - Changed account resources to return raw value rather than prematurely rounding. This was causing issues for some tools which use things like # of keys to determine whether there are enough resources.

## 2.0.x
* Introduced standalone installer which will install required .net dependencies and check for compatibility issues
* Adopted WinUI as default UI provider (old winforms UI can be used by launching with the `--render-engine WinForms` argument)
