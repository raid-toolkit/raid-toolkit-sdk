using Raid.Toolkit.Common.API;
using Raid.Toolkit.Extensibility.Interfaces;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
	public class ClientMenuManager : MenuManager
	{
		private readonly IWorkerApplication WorkerApplication;
		public ClientMenuManager(IWorkerApplication workerApplication)
		{
			WorkerApplication = workerApplication;
		}
		private class ClientMenuApi : ApiCallerBase<IMenuManagerApi>, IMenuManagerApi
		{
			public ClientMenuApi(IWorkerApplication workerApplication)
				: base(workerApplication.Client)
			{ }

			public event EventHandler<string>? EntryActivated;

			public Task<bool> AddEntry(string id, string displayName)
			{
				return CallMethod<bool>(MethodBase.GetCurrentMethod(), id, displayName);
			}

			public Task<bool> RemoveEntry(string id)
			{
				return CallMethod<bool>(MethodBase.GetCurrentMethod(), id);
			}
		}
	}
}
