using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common.API;

namespace Raid.Toolkit.Extensibility.Host.Server;

public class RuntimeManagerServer : ApiServer<IRuntimeManager>, IRuntimeManager, IHostedService
{
	private class ProcessState
	{
		public int ProcessId { get; set; }
		[MemberNotNullWhen(true, nameof(Descriptor), nameof(RuntimeContext))]
		public bool IsInjected => RuntimeContext != null && Descriptor != null;
		public InjectedRuntimeDescriptor? Descriptor { get; set; }
		public Il2CsRuntimeContext? RuntimeContext { get; set; }
	}

	private readonly Dictionary<int, ProcessState> ProcessStates = new();
	private readonly Dictionary<string, InjectedRuntimeDescriptor> InjectedRuntimes = new();
	private readonly IServerApplication? ServerApplication;
	private bool IsRunning = false;

	public event EventHandler<RuntimeAddedEventArgs>? OnAdded;
	public event EventHandler<RuntimeRemovedEventArgs>? OnRemoved;

	public RuntimeManagerServer(IServiceProvider serviceProvider, ILogger<ApiServer<IRuntimeManager>> logger)
		: base(logger)
	{
		ServerApplication = serviceProvider.GetService<IServerApplication>();
	}

	public Task<InjectedRuntimeDescriptor[]> GetRuntimes()
	{
		return Task.FromResult(InjectedRuntimes.Values.ToArray());
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		ServerApplication?.RegisterApiServer(this);
		IsRunning = true;
		await Task.Run(ServiceWorker, cancellationToken);
	}

	private async void ServiceWorker()
	{
		if (!IsRunning)
			return;

		RefreshProcesses();

		if (IsRunning)
			await Task.Delay(500);

		if (IsRunning)
			_ = Task.Run(ServiceWorker);
	}

	private void RefreshProcesses()
	{
		Process[] processes = Process.GetProcessesByName(Constants.TargetProcessName);
		Dictionary<int, Process> processMap = processes.ToDictionary(proc => proc.Id);

		// Find ProcessStates which have IsInjected==true but are no longer in the current processes
		List<int> removedProcessIds = new();
		foreach (var processState in ProcessStates.Values)
		{
			if (processState.IsInjected && !processMap.ContainsKey(processState.ProcessId))
			{
				removedProcessIds.Add(processState.ProcessId);
				OnRemoved?.Invoke(this, new(processState.Descriptor));
			}
		}

		// Remove from ProcessStates
		foreach (var processId in removedProcessIds)
		{
			ProcessStates.Remove(processId);
		}

		foreach (Process process in processes)
		{
			if (!ProcessStates.TryGetValue(process.Id, out ProcessState? state))
			{
				state = new ProcessState
				{
					ProcessId = process.Id
				};
				ProcessStates.Add(process.Id, state);
			}
			if (state.IsInjected)
				continue;

			state.RuntimeContext = new Il2CsRuntimeContext(process);

			if (!state.IsInjected)
			{
				// V3 TODO: Generate a different ID than process Id?
				Il2CsRuntimeContext context = new(process);
				state.RuntimeContext = context;

				InjectedRuntimeDescriptor descriptor = new(process.Id.ToString(), process.Id);
				state.Descriptor = descriptor;

				InjectedRuntimes.Add(descriptor.Id, descriptor);
				OnAdded?.Invoke(this, new RuntimeAddedEventArgs(descriptor));
			}
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		IsRunning = false;
		return Task.CompletedTask;
	}
}
