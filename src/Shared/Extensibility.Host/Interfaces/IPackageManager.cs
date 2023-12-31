using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
	public enum ExtensionRestartReason
	{
		Installed,
		Updated,
		Removed
	}
	public class PackageModifiedEventArgs : EventArgs
	{
		public string PackageId { get; }
		public ExtensionRestartReason Reason {get;}
		public PackageModifiedEventArgs(string packageId, ExtensionRestartReason reason)
		{
			PackageId = packageId;
			Reason = reason;
		}
	}
	public interface IPackageManager
	{
		public event EventHandler<PackageModifiedEventArgs>? PackageUpdated;
		public string? DebugPackage { get; set; }
		public IEnumerable<ExtensionBundle> GetAllPackages();
		public ExtensionBundle GetPackage(string packageId);
		public ExtensionBundle? InstallPackage(ExtensionBundle package);
		public void RemovePackage(string packageId);
	}
}
