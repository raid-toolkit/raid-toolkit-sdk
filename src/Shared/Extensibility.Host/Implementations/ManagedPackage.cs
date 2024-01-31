using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Helpers;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host;

public class ManagedPackage : IManagedPackage, IExtensionHost, IDisposable
{
	public ExtensionBundle Bundle { get; }
	private readonly IServiceProvider ServiceProvider;
	private readonly ILogger<ManagedPackage> Logger;
	private readonly DependencySynthesizer Dependencies;

	private IMenuManager MenuManager => Dependencies.GetRequiredService<IMenuManager>();
	private IAccountManagerInternals AccountManager => Dependencies.GetRequiredService<IAccountManager>() as IAccountManagerInternals
		?? throw new InvalidOperationException("AccountManager does not implement IAccountManagerInternals");
	private IWindowManager WindowManager => Dependencies.GetRequiredService<IWindowManager>();
	private IServiceManager ServiceManager => Dependencies.GetRequiredService<IServiceManager>();
	private IPackageLoader Loader => Dependencies.GetRequiredService<IPackageLoader>();
	private IModelLoader ModelLoader => Dependencies.GetRequiredService<IModelLoader>();
	private IScopedServiceManager ScopedServices => Dependencies.GetRequiredService<IScopedServiceManager>();

	private IExtensionPackage? ExtensionPackage;
	private bool IsDisposed;

	public PackageState State { get; private set; }

	public ManagedPackage(
		// args
		ExtensionBundle bundle,
		// injected
		IServiceProvider serviceProvider,
		ILogger<ManagedPackage> logger
		)
	{
		State = PackageState.None;
		Bundle = bundle;
		ServiceProvider = serviceProvider;
		Dependencies = new(serviceProvider, true);
		Logger = logger;
	}

	public static ManagedPackage CreateHost(IServiceProvider serviceProvider, ExtensionBundle bundle)
	{
		return ActivatorUtilities.CreateInstance<ManagedPackage>(serviceProvider, bundle);
	}

	public Regex[] GetIncludeTypes()
	{
		if (Bundle.Manifest.Codegen == null)
			return Array.Empty<Regex>();

		Regex[] typePatterns = Bundle.Manifest.Codegen.Types
			.Select(pattern => new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled))
			.ToArray();
		return typePatterns;
	}

	public async Task Load()
	{
		if (State != PackageState.None || ExtensionPackage != null)
		{
			Logger.LogError("The extension {ExtensionId} is in invalid state {State}", Bundle.Id, State);
			return;
		}

		try
		{
			if (string.IsNullOrEmpty(Bundle.Location))
				throw new DirectoryNotFoundException(Bundle.Location);
			Regex[] includeTypes = GetIncludeTypes();
			if (includeTypes.Length > 0)
			{
				await ModelLoader.BuildAndLoad(includeTypes, Bundle.Location);
			}
			ExtensionPackage = await Loader.LoadPackage(Bundle);
			State = PackageState.Loaded;
		}
		catch (TypeLoadException ex)
		{
			State = PackageState.Error;
			Logger.LogError(ExtensionError.TypeLoadFailure.EventId(), ex, "Failed to load extension");
			throw new FileLoadException("Failed to load extension");
		}
		catch (Exception ex)
		{
			Logger.LogError(ExtensionError.FailedToLoad.EventId(), ex, "Failed to load extension");
			throw new FileLoadException("Failed to load extension");
		}
	}

	public void Activate()
	{
		try
		{
			if (State == PackageState.Activated)
				return;

			if (ExtensionPackage == null)
				throw new InvalidOperationException("ExtensionPackage is not set");

			ExtensionPackage.OnActivate(this);
			State = PackageState.Activated;
		}
		catch (Exception ex)
		{
			Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to load extension");
			State = PackageState.Error;
			throw;
		}
	}

	public void Deactivate()
	{
		if (State is not PackageState.Activated and not PackageState.Error)
			return;

		if (ExtensionPackage == null)
			throw new InvalidOperationException("ExtensionPackage is not set");

		ExtensionPackage.OnDeactivate(this);
		State = PackageState.Disabled;
	}

	public void Install()
	{
		if (ExtensionPackage == null)
			throw new InvalidOperationException("ExtensionPackage is not set");

		ExtensionPackage.OnInstall(this);
	}

	public Task Uninstall()
	{
		try
		{
			if (ExtensionPackage == null)
				throw new InvalidOperationException("ExtensionPackage is not set");

			ExtensionPackage.OnUninstall(this);
			State = PackageState.PendingUninstall;
		}
		catch (Exception ex)
		{
			Logger.LogError(ExtensionError.FailedToActivate.EventId(), ex, "Failed to uninstall extension");
			State = PackageState.Error;
		}
		return Task.CompletedTask;
	}

	public void ShowUI()
	{
		if (!CanShowUI)
			throw new ApplicationException("Cannot show user interface");

		if (ExtensionPackage == null)
			throw new InvalidOperationException("ExtensionPackage is not set");

		ExtensionPackage.ShowUI();
	}

	public bool CanShowUI => WindowManager.CanShowUI;

	public int HandleRequest(Uri requestUri)
	{
		try
		{
			return ExtensionPackage?.HandleRequest(requestUri) == true ? 1 : 0;
		}
		catch (Exception ex)
		{
			Logger.LogError(ExtensionError.HandleRequestFailed.EventId(), ex, "Failed to uninstall extension");
			return -2;
		}
	}

	#region IExtensionHost
	public IExtensionStorage GetStorage(bool enableCache)
	{
		IDataStorage storage = enableCache
			? ServiceProvider.GetRequiredService<CachedDataStorage<PersistedDataStorage>>()
			: ServiceProvider.GetRequiredService<PersistedDataStorage>();

		return new ExtensionStorage(storage, new(Bundle.Id));
	}

	public IExtensionStorage GetStorage(IAccount account, bool enableCache)
	{
		IDataStorage storage = enableCache
			? ServiceProvider.GetRequiredService<CachedDataStorage<PersistedDataStorage>>()
			: ServiceProvider.GetRequiredService<PersistedDataStorage>();

		return new ExtensionStorage(storage, new(account.Id, Bundle.Id));
	}

	public T CreateInstance<T>(params object[] args)
	{
		IServiceProvider scope = ServiceProvider.CreateScope().ServiceProvider;
		T instance = ActivatorUtilities.CreateInstance<T>(scope, args);
		return instance;
	}


	public IDisposable RegisterWindow<T>(WindowOptions options) where T : class
	{
		WindowManager.RegisterWindow<T>(options);
		return new HostResourceHandle(() => WindowManager.UnregisterWindow<T>());
	}

	public IWindowAdapter<T> CreateWindow<T>() where T : class
	{
		return WindowManager.CreateWindow<T>();
	}

	public IDisposable RegisterMessageScopeHandler<T>(T handler) where T : IMessageScopeHandler
	{
		ScopedServices.AddMessageScopeHandler(handler);
		return new HostResourceHandle(() => ScopedServices.RemoveMessageScopeHandler(handler));
	}

	public IDisposable RegisterBackgroundService<T>(T service) where T : IBackgroundService
	{
		return ServiceManager.AddService(service);
	}

	public IDisposable RegisterAccountExtension<T>(T factory) where T : IAccountExtensionFactory
	{
		AccountManager.RegisterAccountExtension(Bundle.Manifest, factory);
		return new HostResourceHandle(() => AccountManager.UnregisterAccountExtension(Bundle.Manifest, factory));
	}

	public IEnumerable<IAccount> GetAccounts()
	{
		return AccountManager.Accounts;
	}

	public bool TryGetAccount(string accountId, [NotNullWhen(true)] out IAccount? account)
	{
		return AccountManager.TryGetAccount(accountId, out account);
	}


	public IDisposable RegisterMenuEntry(IMenuEntry entry)
	{
		MenuManager.AddEntry(entry);
		return new HostResourceHandle(() => MenuManager.RemoveEntry(entry));
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Dependencies.Dispose();
			}

			IsDisposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
	#endregion
}
