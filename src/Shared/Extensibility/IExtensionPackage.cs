namespace Raid.Toolkit.Extensibility
{
	public interface IExtensionPackage
	{
        public void OnActivate(IExtensionHost host);
        public void OnDeactivate(IExtensionHost host);
        public void OnInstall(IExtensionHost host);
        public void OnUninstall(IExtensionHost host);
    }
}
