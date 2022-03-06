using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public ProcessManager(IOptions<ProcessManagerSettings> settings)
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
                    ProcessFound?.Invoke(this, args);
                    if (!args.Retry)
                        ActiveProcesses.Add(process.Id, process);
                }
            }
            foreach (int closedProcessId in currentIds)
            {
                if (ActiveProcesses.Remove(closedProcessId, out Process closedProcess))
                {
                    ProcessClosed?.Invoke(this, new IProcessManager.ProcessEventArgs(closedProcessId));
                    closedProcess.Dispose();
                }
            }
        }
    }
}