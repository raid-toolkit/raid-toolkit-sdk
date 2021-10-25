using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class ProcessWatcher : BackgroundService
    {
        public class ProcessWatcherEventArgs : EventArgs
        {
            public int Id { get; }
            public Process Process { get; }
            public bool Retry { get; set; } = false;
            public ProcessWatcherEventArgs(Process process)
            {
                Process = process;
                Id = process.Id;
            }
            public ProcessWatcherEventArgs(int id)
            {
                Id = id;
            }
        }

        private readonly Dictionary<int, Process> ActiveProcesses = new();
        private readonly ProcessWatcherSettings Settings;

        public event EventHandler<ProcessWatcherEventArgs> ProcessFound;
        public event EventHandler<ProcessWatcherEventArgs> ProcessClosed;

        public ProcessWatcher(IOptions<AppSettings> settings) => Settings = settings.Value.ProcessWatcher;

        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RefreshProcesses();
            return Task.Delay(-1, stoppingToken);
        }

        private void RefreshProcesses()
        {
            Process[] processes = Process.GetProcessesByName(Settings.ProcessName);
            HashSet<int> currentIds = new(ActiveProcesses.Keys);
            foreach (Process process in processes)
            {
                currentIds.Remove(process.Id);
                if (!ActiveProcesses.ContainsKey(process.Id))
                {
                    ProcessWatcherEventArgs args = new(process);
                    ProcessFound?.Invoke(this, args);
                    if (!args.Retry)
                        ActiveProcesses.Add(process.Id, process);
                }
            }
            foreach (int closedProcessId in currentIds)
            {
                if (ActiveProcesses.TryGetValue(closedProcessId, out Process closedProcess))
                {
                    ProcessClosed?.Invoke(this, new ProcessWatcherEventArgs(closedProcessId));
                    closedProcess.Dispose();
                }
            }
            TaskExtensions.RunAfter(Settings.PollIntervalMs, RefreshProcesses);
        }
    }
}