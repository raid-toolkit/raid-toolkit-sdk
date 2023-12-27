using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Interfaces;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ClientMenuManager : MenuManager
	{
		private readonly ClientMenuApi MenuClient;
		public ClientMenuManager(IWorkerApplication workerApplication)
		{
			MenuClient = new(workerApplication);
		}

		public override string AddEntry(IMenuEntry entry)
		{
			string id = base.AddEntry(entry);
			MenuClient.AddEntry(id, entry.DisplayName);
			return id;
		}

		public override void RemoveEntry(IMenuEntry entry)
		{
			base.RemoveEntry(entry);
			MenuClient.RemoveEntry(Entries[entry]);
		}

		private class ClientMenuApi : ApiCallerBase<IMenuManagerApi>, IMenuManagerApi
		{
			public ClientMenuApi(IWorkerApplication workerApplication)
				: base(workerApplication.Client)
			{ }

			public event EventHandler<string>? EntryActivated;

			public Task<bool> AddEntry(string id, string displayName)
			{
				return CallMethod<bool>(MethodBase.GetCurrentMethod()!, id, displayName);
			}

			public Task<bool> RemoveEntry(string id)
			{
				return CallMethod<bool>(MethodBase.GetCurrentMethod()!, id);
			}
		}
	}
}
