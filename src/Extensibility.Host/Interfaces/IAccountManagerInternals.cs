namespace Raid.Toolkit.Extensibility.Host;

public interface IAccountManagerInternals : IAccountManager
{
	string ExportAccountData(string accountId);
	void ImportAccountData(string accountData);

	void RegisterAccountExtension<T>(PackageManifest manifest, T factory) where T : IAccountExtensionFactory;
	void UnregisterAccountExtension<T>(PackageManifest manifest, T factory) where T : IAccountExtensionFactory;
}
