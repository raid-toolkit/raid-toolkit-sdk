using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
	public class MenuManager : IMenuManager
	{
		protected readonly List<IMenuEntry> Entries = new();

		public virtual void AddEntry(IMenuEntry entry)
		{
			Entries.Add(entry);
		}

		public IMenuEntry[] GetEntries()
		{
			return Entries.ToArray();
		}

		public virtual void RemoveEntry(IMenuEntry entry)
		{
			Entries.Remove(entry);
		}
	}
}
