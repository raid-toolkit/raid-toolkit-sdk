using Raid.Toolkit.Extensibility.Implementations;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Raid.Toolkit.Extensibility.Host
{
	public abstract class MenuManager : IMenuManager
	{
		protected readonly Dictionary<IMenuEntry, string> Entries = new();

		public virtual void AddEntry(IMenuEntry entry)
		{
			Entries.Add(entry, Guid.NewGuid().ToString("n"));
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
