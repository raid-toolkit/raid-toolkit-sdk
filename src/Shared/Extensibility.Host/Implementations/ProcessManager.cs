using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility
{
    public class ProcessManagerSettings
    {
        public string ProcessName { get; set; }
        public int PollIntervalMs { get; set; }
    }
    public class ProcessManager : IProcessManager
    {
        private readonly Dictionary<int, Process> ActiveProcesses = new();
        private readonly IOptions<ProcessManagerSettings> Settings;
        private readonly ILogger<ProcessManager> Logger;

        public ProcessManager(IOptions<ProcessManagerSettings> settings, ILogger<ProcessManager> logger)
        {
            Settings = settings;
        }

        public event EventHandler<IProcessManager.ProcessEventArgs> ProcessFound;
        public event EventHandler<IProcessManager.ProcessEventArgs> ProcessClosed;

        public void Refresh()
        {
            Process[] processes = Process.GetProcessesByName(Settings.Value.ProcessName);
            HashSet<int> currentIds = new(ActiveProcesses.Keys);
            foreach (Process process in processes)
            {
                _ = currentIds.Remove(process.Id);
                if (!ActiveProcesses.ContainsKey(process.Id))
                {
                    IProcessManager.ProcessEventArgs args = new(process);
                    try
                    {
                        ProcessFound?.Invoke(this, args);
                        if (!args.Retry)
                            ActiveProcesses.Add(process.Id, process);
                    }
                    catch(Exception ex)
                    {
                        Logger.LogWarning("Error thrown in ProcessFound event handler", ex);
                    }
                }
            }
            foreach (int closedProcessId in currentIds)
            {
                if (ActiveProcesses.Remove(closedProcessId, out Process closedProcess))
                {
                    try
                    {
                        ProcessClosed?.Invoke(this, new IProcessManager.ProcessEventArgs(closedProcessId));
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
}
