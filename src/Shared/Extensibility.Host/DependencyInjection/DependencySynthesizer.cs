using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Helpers;

public class DependencySynthesizer : IDisposable
{
	private readonly Dictionary<Type, object?> Dependencies = new();
	private readonly IServiceScope? Scope;
	private readonly IServiceProvider ServiceProvider;
	private bool IsDisposed;

	public DependencySynthesizer(IServiceProvider provider)
	{
		ServiceProvider = provider;
	}

	public DependencySynthesizer(IServiceProvider provider, bool useScope)
	{
		ServiceProvider = provider;
		if (useScope)
		{
			Scope = ServiceProvider.CreateScope();
			ServiceProvider = Scope.ServiceProvider;
		}
	}

	public DependencySynthesizer(IServiceScope scope)
	{
		Scope = scope;
		ServiceProvider = scope.ServiceProvider;
	}

	public T? GetService<T>() where T : class
	{
		if (!Dependencies.TryGetValue(typeof(T), out var dependency))
			Dependencies.Add(typeof(T), dependency = ServiceProvider.GetService(typeof(T)));

		return dependency as T;
	}

	public T GetRequiredService<T>() where T : class
	{
		return GetService<T>() as T ?? throw new NotSupportedException();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing)
			{
				Scope?.Dispose();
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
}
