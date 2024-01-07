using System;
using System.Threading.Tasks;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility;

public record InjectedRuntimeDescriptor(string Id, int ProcessId);

[PublicApi(nameof(IRuntimeManager))]
public interface IRuntimeManager
{
	[PublicApi(nameof(OnAdded))]
	event EventHandler<RuntimeAddedEventArgs> OnAdded;

	[PublicApi(nameof(OnRemoved))]
	event EventHandler<RuntimeRemovedEventArgs> OnRemoved;

	[PublicApi(nameof(GetRuntimes))]
	Task<InjectedRuntimeDescriptor[]> GetRuntimes();
}

public class RuntimeEventArgs : SerializableEventArgs
{
	public InjectedRuntimeDescriptor Descriptor => (InjectedRuntimeDescriptor)EventArguments[0];
	public RuntimeEventArgs(InjectedRuntimeDescriptor descriptor) : base(descriptor) { }
}

public class RuntimeAddedEventArgs : RuntimeEventArgs { public RuntimeAddedEventArgs(InjectedRuntimeDescriptor descriptor) : base(descriptor) { } }
public class RuntimeRemovedEventArgs : RuntimeEventArgs { public RuntimeRemovedEventArgs(InjectedRuntimeDescriptor descriptor) : base(descriptor) { } }
