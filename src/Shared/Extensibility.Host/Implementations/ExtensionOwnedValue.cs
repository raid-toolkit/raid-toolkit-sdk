namespace Raid.Toolkit.Extensibility.Host;

internal class ExtensionOwnedValue<T>
{
	public PackageManifest Manifest;
	public T Value;
	public ExtensionOwnedValue(PackageManifest manifest, T value)
	{
		Manifest = manifest;
		Value = value;
	}
}
