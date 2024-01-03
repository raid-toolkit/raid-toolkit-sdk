using System.Threading.Tasks;

using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extensibility;

public interface IAccountPublicApi<T> where T : class
{
	T GetApi();
}

public interface IAccountExportable
{
	void Export(IAccountReaderWriter account);
	void Import(IAccountReaderWriter account);
}

public interface IAccountExtensionService
{
	bool HasWork { get; }
	Task OnTick();
}

public interface IAccountExtension
{
	void OnConnected(Il2CsRuntimeContext runtime);
	void OnDisconnected();
}

public interface IAccountExtensionFactory
{
	IAccountExtension Create(IAccount account);
}
