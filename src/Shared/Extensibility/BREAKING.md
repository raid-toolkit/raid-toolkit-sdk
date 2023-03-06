
## Breaking changes in v2.4

* Removed deprecated method `IExtensionHost.GetInstance`
* `IPackageLoader.LoadPackage` is now async and returns `Task<IExtensionPackage>`