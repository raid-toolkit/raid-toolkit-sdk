using System.Collections.Generic;
using System.Diagnostics;

namespace Raid.Toolkit.Extensibility.Host
{
    public interface IGameInstanceManager
    {
        public void AddInstance(Process process);
        public void RemoveInstance(int token);
        public IReadOnlyList<IGameInstance> Instances { get; }
    }
}