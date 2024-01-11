# Raid Toolkit 3.x Update Release Notes

## Breaking changes

### `IAppUI` interface deprecated

The `IAppUI` interface was originally created for internal use in the RTK application, but provided vital functionality for executing code on the UI thread.  This interface has been deprecated in favor of the `IAppDispatcher` interface, which provides the same functionality without coupling to RTK Application specific contracts.

The following methods which were intended for internal use only will no longer be available to extensions:

```cs
    void Run();

    Task<bool> ShowExtensionInstaller(ExtensionBundle bundleToInstall);
    void ShowMain();
    void ShowSettings();
    void ShowExtensionManager();
```

#### Upgrade instructions

To update code that is using `IAppUI`, you can simply do a find/replace of `IAppUI` with `IAppDispatcher` (and update your variable names accordingly.)

### Interface Removals

The following interfaces were not intended for use by extensions and have been removed from public libraries:

* `IAppService`
* `IAppUI`
* `IUpdateService`
* `INotificationManager`
