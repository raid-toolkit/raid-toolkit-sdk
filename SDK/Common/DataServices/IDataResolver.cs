namespace Raid.DataServices
{
    public interface IDataResolver<TContext, TData>
        where TContext : class, IDataContext
        where TData : class
    {
        bool TryRead(TContext context, out TData value);
        void Write(TContext context, TData value);
    }
    public interface IDataResolver<TContext, TStorage, TData> : IDataResolver<TContext, TData>
        where TContext : class, IDataContext
        where TStorage : IDataStorage
        where TData : class
    {
    }

    public class DataResolverManager<TContext, TStorage, TData> : IDataResolver<TContext, TStorage, TData>
        where TContext : class, IDataContext
        where TStorage : IDataStorage
        where TData : class
    {
        private readonly IDataStorageFactory<TStorage> Factory;
        private readonly IDataType<TData> DataType;

        public DataResolverManager(IDataStorageFactory<TStorage> factory, IDataType<TData> dataType)
        {
            Factory = factory;
            DataType = dataType;
        }

        public bool TryRead(TContext context, out TData value)
        {
            return Factory.GetStorage(context).TryRead(DataType.Attribute.Key, out value);
        }

        public void Write(TContext context, TData value)
        {
            Factory.GetStorage(context).Write(DataType.Attribute.Key, value);
        }
    }
}
