using System;
using System.Threading.Tasks;

using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility;

public interface IExtensionPackage : IDisposable
{
	Task Load();
	void OnActivate(IExtensionHost host);
	void OnDeactivate(IExtensionHost host);
	void OnInstall(IExtensionHost host);
	void OnUninstall(IExtensionHost host);
	void ShowUI();
	bool HandleRequest(Uri requestUri);
}

public abstract class ExtensionPackage : IExtensionPackage
{
	protected readonly DisposableCollection Disposables = new();
	protected bool IsDisposed;

	public virtual Task Load() => Task.FromResult(0);

	public abstract void OnActivate(IExtensionHost host);
	public virtual void OnDeactivate(IExtensionHost host)
	{
		Disposables.Reset();
	}
	public virtual void OnInstall(IExtensionHost host) { }
	public virtual void OnUninstall(IExtensionHost host) { }
	public virtual void ShowUI() { }
	public virtual bool HandleRequest(Uri requestUri) { return false; }

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Disposables.Dispose();
		}
		IsDisposed = true;
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
