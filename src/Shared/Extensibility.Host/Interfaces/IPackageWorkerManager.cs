using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public interface IManagedPackageWorker : IManagedPackageState
{
	void Start();
	Task Stop();
	void Enable();
	Task Disable();
}
public interface IPackageWorkerManager
{
	bool TryGetPackageWorker(string packageId, [NotNullWhen(true)] out IManagedPackageWorker? extension);
	Task StartExtensions();
	Task StopExtensions();
	void DisablePackage(string packageId);
	void EnablePackage(string packageId);
}
