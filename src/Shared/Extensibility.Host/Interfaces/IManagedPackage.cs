using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public enum PackageState
{
	None,
	Loaded,
	Activated,
	Error,
	Disabled,
	PendingUninstall,
}
public interface IManagedPackageState
{
	PackageState State { get; }
	ExtensionBundle Bundle { get; }
	void Install();
	Task Uninstall();
	void ShowUI();
}
public interface IManagedPackage : IManagedPackageState
{
	Regex[] GetIncludeTypes();
	Task Load();
	void Activate();
	void Deactivate();
	int HandleRequest(Uri requestUri);
}
