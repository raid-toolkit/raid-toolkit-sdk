using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Raid.Service
{
    public class ProcessWatcherService : BackgroundService
    {
        public class ProcessWatcherEventArgs : EventArgs
        {
            public int Id { get; }
            public Process Process { get; }
            public bool Retry { get; set; }
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

        public ProcessWatcherService(IOptions<AppSettings> settings)
        {
            Settings = settings.Value.ProcessWatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000, stoppingToken);
            RefreshProcesses();
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch { }
        }

        private void RefreshProcesses()
        {
            Process[] processes = Process.GetProcessesByName(Settings.ProcessName);
            HashSet<int> currentIds = new(ActiveProcesses.Keys);
            foreach (Process process in processes)
            {
                _ = currentIds.Remove(process.Id);
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
                if (ActiveProcesses.Remove(closedProcessId, out Process closedProcess))
                {
                    ProcessClosed?.Invoke(this, new ProcessWatcherEventArgs(closedProcessId));
                    closedProcess.Dispose();
                }
            }
            _ = TaskExtensions.RunAfter(Settings.PollIntervalMs, RefreshProcesses);
        }
    }
}
