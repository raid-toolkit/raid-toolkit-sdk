namespace Raid.Toolkit.Extensibility;

public interface IMenuManager
{
	void AddEntry(IMenuEntry entry);
	void RemoveEntry(IMenuEntry entry);
	IMenuEntry[] GetEntries();
}
