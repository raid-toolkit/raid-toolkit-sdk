using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Host;

namespace Raid.Toolkit.Extensibility;

public class MenuManagerServer : ApiServer<IMenuManagerApi>, IMenuManagerApi, IMenuManager, IHostedService
{
	private readonly IServerApplication ServerApplication;
	private readonly Dictionary<string, IMenuEntry> Entries = new();

	public MenuManagerServer(IServerApplication serverApplication, ILogger<ApiServer<IMenuManagerApi>> logger)
		: base(logger)
	{
		ServerApplication = serverApplication;
	}

	public IMenuEntry[] GetEntries() => Entries.Values.ToArray();
	public virtual void AddEntry(IMenuEntry entry) => Entries[entry.Id] = entry;
	public virtual void RemoveEntry(IMenuEntry entry) => Entries.Remove(entry.Id);

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public Task StartAsync(CancellationToken cancellationToken)
	{
		ServerApplication?.RegisterApiServer(this);
		return Task.CompletedTask;
	}

	public event EventHandler<MenuEntryActivatedEventArgs>? MenuEntryActivated;

	public Task AddOrUpdateEntry(MenuEntryData entry)
	{
		MenuEntry newEntry = new(entry.Id, entry.DisplayName, entry.IsEnabled, entry.IsVisible, entry.ImageUrl);
		newEntry.Activate += (s, e) => RaiseMenuEntryActivated(entry.Id);
		AddEntry(newEntry);
		return Task.CompletedTask;
	}

	public Task RemoveEntry(string id)
	{
		RemoveEntry(id);
		return Task.CompletedTask;
	}

	internal void RaiseMenuEntryActivated(string menuItemId)
	{
		MenuEntryActivated?.Invoke(this, new MenuEntryActivatedEventArgs(menuItemId));
	}
}
