using System;
using System.Threading.Tasks;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;

namespace Raid.Toolkit.Extensibility;

public class MenuEntryActivatedEventArgs : SerializableEventArgs
{
	public string MenuItemId => (string)EventArguments[0];

	public MenuEntryActivatedEventArgs(string menuItemId)
		: base(menuItemId)
	{
	}
}

public record MenuEntryData(string Id, string DisplayName, bool IsEnabled, bool IsVisible, string? ImageUrl);

[PublicApi(nameof(IMenuManagerApi))]
public interface IMenuManagerApi
{
	[PublicApi(nameof(MenuEntryActivated))]
	event EventHandler<MenuEntryActivatedEventArgs> MenuEntryActivated;

	[PublicApi(nameof(AddOrUpdateEntry))]
	Task AddOrUpdateEntry(MenuEntryData entry);

	[PublicApi(nameof(RemoveEntry))]
	Task RemoveEntry(string id);
}
