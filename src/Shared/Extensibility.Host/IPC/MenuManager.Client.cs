using System;
using System.Reflection;
using System.Threading.Tasks;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility;

public class MenuManagerClient : ApiCallerBase<IMenuManagerApi>, IMenuManagerApi, IMenuManager
{
	public MenuManagerClient(IWorkerApplication workerApplication)
		: base(workerApplication.Client)
	{
	}

	public event EventHandler<MenuEntryActivatedEventArgs> MenuEntryActivated
	{
		add => AddHandler(nameof(MenuEntryActivated), value);
		remove => RemoveHandler(nameof(MenuEntryActivated), value);
	}

	public Task AddOrUpdateEntry(MenuEntryData entry)
	{
		return CallMethod(MethodBase.GetCurrentMethod()!, entry);
	}

	public Task RemoveEntry(string id)
	{
		return CallMethod(MethodBase.GetCurrentMethod()!, id);
	}

	public IMenuEntry[] GetEntries()
	{
		throw new NotSupportedException();
	}

	public void AddEntry(IMenuEntry entry)
	{
		AddOrUpdateEntry(new MenuEntryData(entry.Id, entry.DisplayName, entry.IsEnabled, entry.IsVisible, entry.ImageUrl));
	}

	public void RemoveEntry(IMenuEntry entry)
	{
		RemoveEntry(entry.Id);
	}
}
