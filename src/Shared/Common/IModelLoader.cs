using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit.Common;

public interface IModelLoader
{
	public enum LoadState
	{
		Initialize,
		Rebuild,
		Ready,
		Loaded,
		Error,
	}

	public class TaskProgress
	{
		public string? DisplayName { get; set; }
		public int Completed { get; set; }
		public int Total { get; set; }
	}

	public class ModelLoaderEventArgs : EventArgs
	{
		public TaskProgress? Progress { get; set; }
		public LoadState LoadState { get; set; }
		public ModelLoaderEventArgs(LoadState state, TaskProgress? progress = null) => (LoadState, Progress) = (state, progress);
	}

	public string? GameVersion { get; }
	public event EventHandler<ModelLoaderEventArgs>? OnStateUpdated;

	public Task<Assembly> BuildAndLoad(IEnumerable<Regex> regices, string outputDirectory);
}
