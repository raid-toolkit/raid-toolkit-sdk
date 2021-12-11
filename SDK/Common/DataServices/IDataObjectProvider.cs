namespace Raid.DataServices
{
    public interface IDataObjectProvider
    {
        DataTypeAttribute DataType { get; }
        object GetValue(IDataContext context);
    }

    public interface IDataObjectProvider<TContext, TData> : IDataObjectProvider
        where TContext : class, IDataContext
        where TData : class
    {
        TData GetValue(TContext context);
    }

    public class DataObjectProviderBase<TContext, TData> : IDataObjectProvider<TContext, TData>
        where TContext : class, IDataContext
        where TData : class
    {
        public DataTypeAttribute DataType => PrimaryProvider.DataType.Attribute;

        protected readonly IDataResolver<TContext, TData> PrimaryProvider;

        public DataObjectProviderBase(IDataResolver<TContext, TData> primaryProvider)
        {
            PrimaryProvider = primaryProvider;
        }

        public virtual TData GetValue(TContext context)
        {
            return PrimaryProvider.TryRead(context, out TData value) ? value : default;
        }

        object IDataObjectProvider.GetValue(IDataContext context)
        {
            return GetValue((TContext)context);
        }
    }
}