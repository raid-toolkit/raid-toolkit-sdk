using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility;

public class ProcessManagerSettings
{
	public string ProcessName { get; set; } = "Raid";
	public int PollIntervalMs { get; set; } = 1000;
}
public class ProcessManager : IProcessManager
{
	private readonly Dictionary<int, Process> ActiveProcesses = new();
	private readonly IOptions<ProcessManagerSettings> Settings;
	private readonly ILogger<ProcessManager> Logger;

	public ProcessManager(IOptions<ProcessManagerSettings> settings, ILogger<ProcessManager> logger)
	{
		Settings = settings;
		Logger = logger;
	}

	public event EventHandler<ProcessEventArgs>? ProcessFound;
	public event EventHandler<ProcessEventArgs>? ProcessClosed;

	public void Refresh()
	{
		Process[] processes = Process.GetProcessesByName(Settings.Value.ProcessName);
		HashSet<int> currentIds = new(ActiveProcesses.Keys);
		foreach (Process process in processes)
		{
			_ = currentIds.Remove(process.Id);
			if (!ActiveProcesses.ContainsKey(process.Id))
			{
				ProcessEventArgs args = new(process);
				try
				{
					ProcessFound?.Raise(this, args);
					if (!args.Retry)
						ActiveProcesses.Add(process.Id, process);
				}
				catch (Exception ex)
				{
					Logger.LogWarning("Error thrown in ProcessFound event handler", ex);
				}
			}
		}
		foreach (int closedProcessId in currentIds)
		{
			if (ActiveProcesses.Remove(closedProcessId, out Process? closedProcess))
			{
				try
				{
					ProcessClosed?.Raise(this, new ProcessEventArgs(closedProcessId));
					closedProcess.Dispose();
				}
				catch (Exception ex)
				{
					Logger.LogWarning("Error thrown in ProcessClosed event handler", ex);
				}
			}
		}
	}
}
