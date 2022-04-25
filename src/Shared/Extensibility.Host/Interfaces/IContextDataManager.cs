using Raid.Toolkit.Extensibility.Providers;
using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public interface IContextDataManager
    {
        public IEnumerable<IDataProvider<TContext>> OfType<TContext>() where TContext : class, IDataContext;
        [Obsolete]
        public IDisposable AddProvider<T>() where T : IDataProvider;
        public IDisposable AddProvider<T>(T provider) where T : IDataProvider;
    }
}
