using Raid.Toolkit.Extensibility.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Raid.Toolkit.Extensibility.Host
{
	public abstract class MenuManager : IMenuManager
	{
		protected readonly Dictionary<IMenuEntry, string> Entries = new();

		public virtual string AddEntry(IMenuEntry entry)
		{
			string id = Guid.NewGuid().ToString("n");
			Entries.Add(entry, id);
			return id;
		}

		public IMenuEntry[] GetEntries()
		{
			return Entries.Keys.ToArray();
		}

		public virtual void RemoveEntry(IMenuEntry entry)
		{
			Entries.Remove(entry);
		}
	}
}
