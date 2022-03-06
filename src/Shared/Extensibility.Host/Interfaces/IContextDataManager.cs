using Il2CppToolkit.Runtime;
using Raid.Toolkit.Extensibility.Providers;
using System;

namespace Raid.Toolkit.Extensibility
{
    public interface IContextDataManager
    {
        public IDisposable AddProvider<T>() where T : IDataProvider;
        public bool Update<T>(Il2CsRuntimeContext runtime, T context) where T : IDataContext;
    }
}
