namespace Raid.Toolkit.Extensibility;

public interface IWindowManager
{
	void RegisterWindow<T>(WindowOptions options) where T : class;
	void UnregisterWindow<T>() where T : class;
	IWindowAdapter<T> CreateWindow<T>() where T : class;
	void RestoreWindows();
	bool CanShowUI { get; }
}
