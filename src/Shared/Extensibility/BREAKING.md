# Breaking changes log

## New APIs in 3.0

### `IAppDispatcher`

The `IAppDispatcher` interface provides a way to execute code on the UI thread.  This interface is intended to replace the `IAppUI` interface, which was originally created for internal use in the RTK application, but provided vital functionality for executing code on the UI thread.  This interface provides the same functionality without coupling to RTK Application specific contracts.

### `IWindowManager`

The `IWindowManager` interface provides a way to create and manage windows.  This interface is intended to replace methods on the `IExtensionHost` interface for a more modular API surface.

## Breaking changes in 3.0

### TODO

* Backport `IAppDispatcher` from 3.0

### `IExtensionHost` removed APIs

#### Window APIs

`RegisterWindow` and `CreateWindow` have been removed.
Use [`IWindowManager`](#iwindowmanager) instead.

<details>
<summary>👩‍💻 Upgrade instructions</summary>

##### Before

```cs
class MyExtension : ExtensionPackage
{
    public override void OnActivate(IExtensionHost host)
    {
        host.RegisterWindow<MyWindow>(new WindowOptions() { /*...*/ });
        host.CreateWindow<MyWindow>();
    }
}
```

##### After

```cs
class MyExtension : ExtensionPackage
{
    private readonly IWindowManager WindowManager;
    public MyExtension(IWindowManager windowManager)
    {
        WindowManager = windowManager;
    }
    public override void OnActivate(IExtensionHost _unused_)
    {
        WindowManager.RegisterWindow<MyWindow>(new WindowOptions() { /*...*/ });
        host.CreateWindow<MyWindow>();
    }
}
```

</details>

### `IAppUI` interface deprecated

The `IAppUI` interface was originally created for internal use in the RTK application, but provided vital functionality for executing code on the UI thread.  This interface has been deprecated in favor of the [`IAppDispatcher`](#iappdispatcher) interface, which provides the same functionality without coupling to RTK Application specific contracts.

The following methods which were intended for internal use only will not be available to extensions via the `IAppDispatcher` interface:

```cs
    void Run();

    Task<bool> ShowExtensionInstaller(ExtensionBundle bundleToInstall);
    void ShowMain();
    void ShowSettings();
    void ShowExtensionManager();
```

<details>
<summary>👩‍💻 Upgrade instructions</summary>

To update code that is using `IAppUI`, you can simply do a find/replace of `IAppUI` with `IAppDispatcher` (and update your variable names accordingly.)

</details>

### Interface Removals

The following interfaces were not intended for use by extensions and have been removed from public libraries:

* `IAppService`
* `IAppUI` (see [IAppUI interface deprecated](#iappui-interface-deprecated))
* `IUpdateService`
* `INotificationManager`, `INotificationSink`, `INotification`
* `IGameInstanceManager`

### Namespace changes

The following namespaces have been changed:

| Old namespace | New namespace |
|:--------------|:--------------|
|Raid.Toolkit.Extensibility.Host.IWindowManager | Raid.Toolkit.Extensibility.IWindowManager |

## Breaking changes in v2.4

* Removed deprecated method `IExtensionHost.GetInstance`
* `IPackageLoader.LoadPackage` is now async and returns `Task<IExtensionPackage>`
