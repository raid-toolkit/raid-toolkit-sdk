using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.Host;

public class GameInstanceManager : PollingBackgroundService, IGameInstanceManager
{
	private readonly ConcurrentDictionary<int, ILoadedGameInstance> _Instances = new();
	private readonly ConcurrentDictionary<int, IGameInstance> _RawInstances = new();
	private readonly IServiceProvider ServiceProvider;
	private readonly IRuntimeManager RuntimeManager;
	private readonly IModelLoader ModelLoader;

	public IReadOnlyList<ILoadedGameInstance> Instances => _Instances.Values.ToList();
	public event EventHandler<GameInstanceAddedEventArgs>? OnAdded;
	public event EventHandler<GameInstanceRemovedEventArgs>? OnRemoved;

	public GameInstanceManager(
		IServiceProvider serviceProvider,
		IModelLoader modelLoader,
		ILogger<IGameInstanceManager> logger,
		IRuntimeManager runtimeManager)
		: base(logger)
	{
		ServiceProvider = serviceProvider;
		RuntimeManager = runtimeManager;
		ModelLoader = modelLoader;
	}

	private void RuntimeManager_OnAdded(object? sender, RuntimeAddedEventArgs e)
	{
		Process proc = Process.GetProcessById(e.Descriptor.ProcessId);
		AddInstance(proc);
	}

	private void RuntimeManager_OnRemoved(object? sender, RuntimeRemovedEventArgs e)
	{
		RemoveInstance(e.Descriptor.ProcessId);
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		var runtimes = await RuntimeManager.GetRuntimes();
		RuntimeManager.OnAdded += RuntimeManager_OnAdded;
		RuntimeManager.OnRemoved += RuntimeManager_OnRemoved;
		foreach (var runtime in runtimes)
		{
			Process proc = Process.GetProcessById(runtime.ProcessId);
			AddInstance(proc);
		}
		await base.StartAsync(cancellationToken);
	}

	protected override Task ExecuteOnceAsync(CancellationToken token)
	{
		if (!ModelLoader.IsLoaded)
			return Task.CompletedTask;

		foreach (var instance in _RawInstances.Values)
		{
			if (_Instances.ContainsKey(instance.Token))
				continue;
			try
			{
				Process process = Process.GetProcessById(instance.Token); // TODO: don't assume token == ProcessId
				ILoadedGameInstance loadedInstance = instance.InitializeOrThrow(process);

				_ = _Instances.TryAdd(instance.Token, loadedInstance);
				OnAdded?.Raise(this, new(loadedInstance));
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to initialize instance {instanceId}", instance.Token);
			}
		}
		return Task.CompletedTask;
	}

	public IGameInstance? GetById(string id)
	{
		return Instances.FirstOrDefault(instance => instance.Id == id);
	}

	public bool TryGetById(string id, [NotNullWhen(true)] out ILoadedGameInstance? instance)
	{
		instance = Instances.FirstOrDefault(instance => instance.Id == id);
		return instance != null;
	}

	private void AddInstance(Process process)
	{
		_RawInstances.GetOrAdd(process.Id, (token) => ActivatorUtilities.CreateInstance<GameInstance>(ServiceProvider, process));
	}

	private void RemoveInstance(int token)
	{
		if (_RawInstances.TryRemove(token, out IGameInstance? instance))
		{
			if (_Instances.TryRemove(token, out ILoadedGameInstance? loadedInstance))
				OnRemoved?.Raise(this, new(instance, loadedInstance.Id));
			instance.Dispose();
		}
	}
}
