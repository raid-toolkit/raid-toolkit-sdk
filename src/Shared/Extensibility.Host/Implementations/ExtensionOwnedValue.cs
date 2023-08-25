namespace Raid.Toolkit.Extensibility.Host
{
    internal class ExtensionOwnedValue<T>
    {
        public ExtensionManifest Manifest;
        public T Value;
        public ExtensionOwnedValue(ExtensionManifest manifest, T value)
        {
            Manifest = manifest;
            Value = value;
        }
    }
}
