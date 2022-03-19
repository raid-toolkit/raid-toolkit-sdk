using Il2CppToolkit.Runtime;
using System;

namespace Raid.Toolkit.Extensibility.Providers
{
    public interface IDataProvider
    {
        public string Key { get; }
        public Version Version { get; }
        public Type ContextType { get; }
        public Type DataType { get; }

        bool Upgrade(IDataContext context, Version dataVersion);
        bool Update(Il2CsRuntimeContext scope, IDataContext context);
    }

    public interface IDataProvider<TContext> : IDataProvider
        where TContext : class, IDataContext
    {
        bool Upgrade(TContext context, Version dataVersion);
        bool Update(Il2CsRuntimeContext scope, TContext context);
    }

    public abstract class DataProvider<TContext, TData> : IDataProvider<TContext>
        where TContext : class, IDataContext
        where TData : class
    {
        private static Version DefaultVersion = new(1, 0);

        public abstract string Key { get; }
        public virtual Version Version => DefaultVersion;
        public Type ContextType => typeof(TContext);
        public Type DataType => typeof(TData);


        public virtual bool Upgrade(TContext context, Version dataVersion)
        {
            return false;
        }

        public abstract bool Update(Il2CsRuntimeContext scope, TContext context);

        bool IDataProvider.Upgrade(IDataContext context, Version dataVersion)
        {
            return Upgrade(context as TContext, dataVersion);
        }

        bool IDataProvider.Update(Il2CsRuntimeContext scope, IDataContext context)
        {
            return Update(scope, context as TContext);
        }
    }
}
