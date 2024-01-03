namespace Raid.Toolkit;

public interface IAppUI : IDisposable
{
	void Run();

	void ShowMain();
	void ShowSettings();
	void ShowExtensionManager();
}
