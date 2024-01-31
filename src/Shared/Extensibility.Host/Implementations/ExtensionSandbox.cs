using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

using CustomExtensions.WinUI;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit.Extensibility.Host;

public class ExtensionSandbox : IDisposable, IExtensionPackage
{
	private bool IsDisposed;
	private readonly ExtensionBundle Bundle;
	private readonly IManagedPackageFactory InstanceFactory;
	private readonly ILogger<ExtensionSandbox> Logger;
	private ExtensionLoadContext? LoadContext;
	private IExtensionPackage? Instance;
	private IExtensionAssembly? ExtensionAsm;

	public PackageManifest Manifest => Bundle.Manifest;

	[ActivatorUtilitiesConstructor]
	public ExtensionSandbox(
		IManagedPackageFactory instanceFactory,
		ILogger<ExtensionSandbox> logger,
		ExtensionBundle bundle)
	{
		Logger = logger;
		InstanceFactory = instanceFactory;
		Bundle = bundle;
		if (!bundle.IsLinkedAssembly)
		{
			if (string.IsNullOrEmpty(bundle.Location))
				throw new EntryPointNotFoundException("Bundle location not set");
			LoadContext = new(bundle.Location);
		}
	}

	private Type GetPackageType()
	{
		Assembly? assembly = (ExtensionAsm?.ForeignAssembly ?? Bundle.Assembly) ?? throw new InvalidOperationException("Extension must be loaded first");
		Type? packageType = assembly.GetType(Manifest.Type);
		return packageType ?? throw new EntryPointNotFoundException($"Could not load type {Manifest.Type} from the package");
	}

	public async Task Load()
	{
		Logger.LogInformation("Loading extension {ManifestId}", Manifest.Id);
		if (!Bundle.IsLinkedAssembly)
		{
			ExtensionAsm ??= await ApplicationExtensionHost.Current.LoadExtensionAsync(Bundle.GetExtensionEntrypointDll());
		}
		await EnsureInstance().Load();
	}

	[MemberNotNull("Instance")]
	private IExtensionPackage EnsureInstance()
	{
		return Instance ??= InstanceFactory.CreateInstance(GetPackageType());
	}

	public void OnActivate(IExtensionHost host)
	{
		Logger.LogInformation("Activating extension {Manifest.Id}", Manifest.Id);
		try
		{
			EnsureInstance().OnActivate(host);
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Activation error {Manifest.Id}", Manifest.Id);
			OnDeactivate(host);
			Dispose();
		}
	}

	public void OnDeactivate(IExtensionHost host)
	{
		if (IsDisposed)
			return;
		Logger.LogInformation("Deactivating extension {Manifest.Id}", Manifest.Id);
		try
		{
			EnsureInstance().OnDeactivate(host);
		}
		catch (Exception e)
		{
			Logger.LogError(e, "Deactivation error {Manifest.Id}", Manifest.Id);
		}
	}

	public void OnInstall(IExtensionHost host)
	{
		if (IsDisposed)
			return;
		Logger.LogInformation("Installing extension {Manifest.Id}", Manifest.Id);
		EnsureInstance().OnInstall(host);
	}

	public void OnUninstall(IExtensionHost host)
	{
		if (IsDisposed)
			return;
		Logger.LogInformation("Uninstalling extension {Manifest.Id}", Manifest.Id);
		EnsureInstance().OnUninstall(host);
	}

	public void ShowUI()
	{
		if (IsDisposed)
			return;
		Logger.LogInformation("Showing extension UI {Manifest.Id}", Manifest.Id);
		EnsureInstance().ShowUI();
	}

	public bool HandleRequest(Uri requestUri)
	{
		if (IsDisposed)
			return false;
		Logger.LogInformation("Forwarding activation request {Manifest.Id} ({requestUri})", Manifest.Id, requestUri);
		return Instance?.HandleRequest(requestUri) ?? false;
	}

	#region IDisposable
	protected virtual void Dispose(bool disposing)
	{
		if (IsDisposed)
			return;
		if (disposing)
		{
			Logger.LogInformation("Disposing extension {Manifest.Id}", Manifest.Id);
			Instance?.Dispose();
			LoadContext?.Unload();
			ExtensionAsm?.Dispose();
		}

		Instance = null;
		ExtensionAsm = null;
		LoadContext = null;
		ExtensionAsm = null;
		IsDisposed = true;
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
