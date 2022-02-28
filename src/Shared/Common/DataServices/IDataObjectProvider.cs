using System;

namespace Raid.DataServices
{
    public interface IDataObjectProvider
    {
		Type ContextType { get; }
        DataTypeAttribute DataType { get; }
        object GetValue(IDataContext_deprecated context);
    }

    public interface IDataObjectProvider<TContext, TData> : IDataObjectProvider
        where TContext : class, IDataContext_deprecated
        where TData : class
    {
        TData GetValue(TContext context);
    }

    public class DataObjectProviderBase<TContext, TData>
        where TContext : class, IDataContext_deprecated
        where TData : class
    {
        public DataTypeAttribute DataType => PrimaryProvider.DataType.Attribute;
        public Type ContextType => typeof(TContext);

        protected readonly IDataResolver<TContext, TData> PrimaryProvider;

        public DataObjectProviderBase(IDataResolver<TContext, TData> primaryProvider)
        {
            PrimaryProvider = primaryProvider;
        }

        public virtual TData GetValue(TContext context)
        {
            return PrimaryProvider.TryRead(context, out TData value) ? value : default;
        }

        public object GetValue(IDataContext_deprecated context)
        {
            return GetValue((TContext)context);
        }
    }
}