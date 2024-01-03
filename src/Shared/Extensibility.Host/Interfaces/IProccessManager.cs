using System;
using System.Diagnostics;

namespace Raid.Toolkit.Extensibility.Host;

public interface IProcessManager
{
	public class ProcessEventArgs : EventArgs
	{
		public int Id { get; }
		public Process? Process { get; }
		public bool Retry { get; set; }
		public ProcessEventArgs(Process process)
		{
			Process = process;
			Id = process.Id;
		}
		public ProcessEventArgs(int id)
		{
			Id = id;
		}
	}

	event EventHandler<ProcessEventArgs>? ProcessFound;
	event EventHandler<ProcessEventArgs>? ProcessClosed;

	void Refresh();
}
