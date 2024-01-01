using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
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
		public PackageState State { get; }
		public ExtensionBundle Bundle { get; }
		public void Install();
		public Task Uninstall();
		public void ShowUI();
	}
	public interface IManagedPackage : IManagedPackageState
	{
        public Regex[] GetIncludeTypes();
        public Task Load();
        public void Activate();
        public void Deactivate();
        public int HandleRequest(Uri requestUri);
	}
}
