using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility;

public class MenuManagerClient : ApiCallerBase<IMenuManagerApi>, IMenuManagerApi, IMenuManager
{
	private Dictionary<string, IMenuEntry> Entries = new();
	public MenuManagerClient(IWorkerApplication workerApplication)
		: base(workerApplication.Client)
	{
		MenuEntryActivated += (s, e) =>
		{
			if (Entries.TryGetValue(e.MenuItemId, out IMenuEntry? entry))
			{
				entry.OnActivate();
			}
		};
	}

	public event EventHandler<MenuEntryActivatedEventArgs> MenuEntryActivated
	{
		add => AddHandler(nameof(MenuEntryActivated), value);
		remove => RemoveHandler(nameof(MenuEntryActivated), value);
	}

	public Task AddOrUpdateEntry(MenuEntryData entry)
	{
		return CallMethod(nameof(AddOrUpdateEntry), entry);
	}

	public Task RemoveEntry(string id)
	{
		return CallMethod(nameof(RemoveEntry), id);
	}

	public IMenuEntry[] GetEntries()
	{
		throw new NotSupportedException();
	}

	public void AddEntry(IMenuEntry entry)
	{
		Entries[entry.Id] = entry;
		AddOrUpdateEntry(new MenuEntryData(entry.Id, entry.DisplayName, entry.IsEnabled, entry.IsVisible, entry.ImageUrl));
	}

	public void RemoveEntry(IMenuEntry entry)
	{
		Entries.Remove(entry.Id);
		RemoveEntry(entry.Id);
	}
}
