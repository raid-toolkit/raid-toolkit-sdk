using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility.Host
{
    public class MenuManager : IMenuManager
    {
        private readonly List<IMenuEntry> Entries = new();

        public void AddEntry(IMenuEntry entry)
        {
            Entries.Add(entry);
        }

        public IMenuEntry[] GetEntries()
        {
            return Entries.ToArray();
        }

        public void RemoveEntry(IMenuEntry entry)
        {
            Entries.Remove(entry);
        }
    }
}
