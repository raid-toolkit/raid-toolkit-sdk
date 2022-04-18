using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility
{
    public interface IMenuManager
    {
        public void AddEntry(IMenuEntry entry);
        public void RemoveEntry(IMenuEntry entry);
        public IMenuEntry[] GetEntries();
    }
}
