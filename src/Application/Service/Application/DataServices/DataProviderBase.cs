using System;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public interface IContextDataProvider { }
    public interface IContextDataProvider<TContext> : IContextDataProvider
        where TContext : class, IDataContext
    {
        DataTypeAttribute DataType { get; }
        bool Upgrade(TContext context, Version dataVersion);
        bool Update(ModelScope scope, TContext context);
    }

    public abstract class DataProviderBase<TContext, TData> :
        DataObjectProviderBase<TContext, TData>,
        IContextDataProvider<TContext>
        where TData : class
        where TContext : class, IDataContext
    {
        public DataProviderBase(IDataResolver<TContext, CachedDataStorage<PersistedDataStorage>, TData> storage)
            : base(storage)
        {
        }

        public virtual bool Upgrade(TContext context, Version dataVersion)
        {
            return false;
        }

        public abstract bool Update(ModelScope scope, TContext context);
    }
}
