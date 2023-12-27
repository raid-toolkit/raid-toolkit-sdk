using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Interfaces;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ServerMenuManager : MenuManager
	{
		private readonly IServerApplication ServerApplication;
		public ServerMenuManager(IServerApplication serverApplication, IServiceProvider serviceProvider)
		{
			ServerApplication = serverApplication;
			MenuServerApi api = ActivatorUtilities.CreateInstance<MenuServerApi>(serviceProvider, this);
			ServerApplication.RegisterApiServer(api);
		}

		private class MenuServerApi : ApiServer<IMenuManagerApi>, IMenuManagerApi
		{
			private readonly ServerMenuManager Menu;

			public MenuServerApi(ILogger<MenuServerApi> logger, ServerMenuManager menu)
				: base(logger) => Menu = menu;

			public event EventHandler<string>? EntryActivated;

			public Task<bool> AddEntry(string id, string displayName)
			{
				MenuEntry entry = new(displayName);
				entry.Activate += (sender, args) => EntryActivated?.Invoke(sender, id);
				Menu.Entries.Add(new MenuEntry(displayName), id);
				return Task.FromResult(true);
			}

			public Task<bool> RemoveEntry(string id)
			{
				var entryToRemove = Menu.Entries.FirstOrDefault(kvp => kvp.Value == id);
				if (entryToRemove.Key == null)
					return Task.FromResult(false);

				Menu.Entries.Remove(entryToRemove.Key);
				return Task.FromResult(true);
			}
		}
	}
}
