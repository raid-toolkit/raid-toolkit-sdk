using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host;

public enum ExtensionRestartReason
{
	Installed,
	Updated,
	Removed
}
public class PackageModifiedEventArgs : EventArgs
{
	public string PackageId { get; }
	public ExtensionRestartReason Reason { get; }
	public PackageModifiedEventArgs(string packageId, ExtensionRestartReason reason)
	{
		PackageId = packageId;
		Reason = reason;
	}
}
public interface IPackageManager
{
	event EventHandler<PackageModifiedEventArgs>? PackageUpdated;
	string? DebugPackage { get; set; }
	IEnumerable<ExtensionBundle> GetAllPackages();
	ExtensionBundle GetPackage(string packageId);
	ExtensionBundle? InstallPackage(ExtensionBundle package);
	void RemovePackage(string packageId);
}
