using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Raid.Service
{
    public class ProcessWatcher
    {
        public class ProcessWatcherEventArgs : EventArgs
        {
            public int Id { get; }
            public Process Process { get; }
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

        private readonly Dictionary<int, Process> m_processes = new();
        private readonly string m_processName;
        private readonly int m_pollIntervalMs;

        public event EventHandler<ProcessWatcherEventArgs> ProcessFound;
        public event EventHandler<ProcessWatcherEventArgs> ProcessClosed;

        public ProcessWatcher(string processName, int pollIntervalMs = 10000)
        {
            m_pollIntervalMs = pollIntervalMs;
            m_processName = processName;
            TaskExtensions.RunAfter(1, RefreshProcesses);
        }

        private void RefreshProcesses()
        {
            Process[] processes = Process.GetProcessesByName("Raid");
            HashSet<int> currentIds = new(m_processes.Keys);
            foreach (Process process in processes)
            {
                currentIds.Remove(process.Id);
                if (!m_processes.ContainsKey(process.Id))
                {
                    m_processes.Add(process.Id, process);
                    ProcessFound?.Invoke(this, new ProcessWatcherEventArgs(process));
                }
            }
            foreach (int closedProcessId in currentIds)
            {
                if (m_processes.TryGetValue(closedProcessId, out Process closedProcess))
                {
                    ProcessClosed?.Invoke(this, new ProcessWatcherEventArgs(closedProcessId));
                    closedProcess.Dispose();
                }
            }
            TaskExtensions.RunAfter(m_pollIntervalMs, RefreshProcesses);
        }
    }
}