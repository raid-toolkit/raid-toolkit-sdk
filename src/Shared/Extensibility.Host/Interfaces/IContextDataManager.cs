using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public interface IContextDataManager
    {
        public IEnumerable<IDataProvider<TContext>> OfType<TContext>() where TContext : class, IDataContext;
        public IDisposable AddProvider<T>() where T : IDataProvider;
    }
}
