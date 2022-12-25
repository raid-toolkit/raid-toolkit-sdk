# Extension development troubleshooting guide

---

## ⛔ **ISSUE**: `ERROR MSB4062` when building extensions with `dotnet build`:

> ```log
> Error MSB4062: The "Microsoft.Build.AppxPackage.GetSdkFileFullPath" task could not be loaded... 
> ```

Add the following to your `.csproj` file in the top `<PropertyGroup>` element:

```xml
<PropertyGroup>
	<!-- ... other properties here ... -->
	<EnablePreviewMsixTooling>true</EnablePreviewMsixTooling>
</PropertyGroup>
```

See [maui/#5886](https://github.com/dotnet/maui/issues/5886#issuecomment-1123106200) for more information.

---