using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Raid.Toolkit.Common;

public enum ModelLoaderState
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
	public ModelLoaderState LoadState { get; set; }
	public ModelLoaderEventArgs(ModelLoaderState state, TaskProgress? progress = null) => (LoadState, Progress) = (state, progress);
}

public class ModelLoaderOptions
{
	public bool ForceRebuild { get; set; } = false;
}

public interface IModelLoader
{
	bool IsLoaded { get; }
	string? GameVersion { get; }
	event EventHandler<ModelLoaderEventArgs>? OnStateUpdated;

	Task<Assembly> BuildAndLoad(IEnumerable<Regex> regices, string outputDirectory);
}
