using Raid.Toolkit.Common.API;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
	[PublicApi(nameof(IMenuManagerApi))]
	public interface IMenuManagerApi
	{
		[PublicApi(nameof(EntryActivated))]
		public event EventHandler<string>? EntryActivated;

		[PublicApi(nameof(AddEntry))]
		public Task<bool> AddEntry(string id, string displayName);

		[PublicApi(nameof(RemoveEntry))]
		public Task<bool> RemoveEntry(string id);
	}
	public interface IMenuManager
	{
		public string AddEntry(IMenuEntry entry);
		public void RemoveEntry(IMenuEntry entry);
		public IMenuEntry[] GetEntries();
	}
}
