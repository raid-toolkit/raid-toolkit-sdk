using Raid.Toolkit.Common.API;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public class RuntimeManagerClient : ApiCallerBase<IRuntimeManager>, IRuntimeManager
{
	public RuntimeManagerClient(IWorkerApplication workerApplication)
		: base(workerApplication.Client)
	{
	}

	public event EventHandler<RuntimeAddedEventArgs> OnAdded
	{
		add => AddHandler(nameof(OnAdded), value);
		remove => RemoveHandler(nameof(OnAdded), value);
	}
	public event EventHandler<RuntimeRemovedEventArgs> OnRemoved
	{
		add => AddHandler(nameof(OnRemoved), value);
		remove => RemoveHandler(nameof(OnRemoved), value);
	}

	public Task<InjectedRuntimeDescriptor[]> GetRuntimes()
	{
		return CallMethod<InjectedRuntimeDescriptor[]>(nameof(GetRuntimes));
	}
}
