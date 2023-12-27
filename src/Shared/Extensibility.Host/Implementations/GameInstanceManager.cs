using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Common;

namespace Raid.Toolkit.Extensibility.Host
{
	public class GameInstanceManager : IGameInstanceManager
	{
		private readonly ConcurrentDictionary<int, ILoadedGameInstance> _Instances = new();
		private readonly ConcurrentDictionary<int, IGameInstance> _RawInstances = new();
		private readonly IServiceProvider ServiceProvider;

		public IReadOnlyList<ILoadedGameInstance> Instances => _Instances.Values.ToList();
		public event EventHandler<IGameInstanceManager.GameInstanceAddedEventArgs>? OnAdded;
		public event EventHandler<IGameInstanceManager.GameInstanceRemovedEventArgs>? OnRemoved;

		public GameInstanceManager(
			IServiceProvider serviceProvider,
			IHostApplicationLifetime lifetime)
		{
			ServiceProvider = serviceProvider;
			_ = lifetime.ApplicationStopped.Register(() =>
			{
				int[] instanceKeys = _RawInstances.Keys.ToArray();
				foreach (int key in instanceKeys)
				{
					_ = _RawInstances.Remove(key, out IGameInstance? instance);
					_ = _Instances.Remove(key, out _);
					instance?.Dispose();
				}
			});
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


		public void AddInstance(Process process)
		{
			IGameInstance instance = _RawInstances.GetOrAdd(process.Id, (token) => ActivatorUtilities.CreateInstance<GameInstance>(ServiceProvider, process));
			ILoadedGameInstance loadedInstance = instance.InitializeOrThrow(process);

			_ = _Instances.TryAdd(instance.Token, loadedInstance);
			OnAdded?.Raise(this, new(loadedInstance));
		}

		public void RemoveInstance(int token)
		{
			if (_RawInstances.TryRemove(token, out IGameInstance? instance))
			{
				if (_Instances.TryRemove(token, out ILoadedGameInstance? loadedInstance))
					OnRemoved?.Raise(this, new(instance, loadedInstance.Id));
				instance.Dispose();
			}
		}
	}
}
