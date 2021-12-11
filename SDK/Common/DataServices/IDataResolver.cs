namespace Raid.DataServices
{
    public interface IDataResolver<TData>
        where TData : class
    {
        bool TryRead(out TData value);
        void Write(TData value);
    }
    public interface IDataResolver<C, T, TData> : IDataResolver<TData>
        where C : IDataContext
        where T : IDataStorage
        where TData : class
    {
    }

    public class DataResolverManager<TContext, TStorage, TData> : IDataResolver<TContext, TStorage, TData>
        where TContext : class, IDataContext
        where TStorage : IDataStorage
        where TData : class
    {
        private readonly IDataContext Context;
        private readonly IDataStorageFactory<TStorage> Factory;
        private readonly IDataType<TData> DataType;

        public DataResolverManager(IContext<TContext> context, IDataStorageFactory<TStorage> factory, IDataType<TData> dataType)
        {
            Context = context.Value;
            Factory = factory;
            DataType = dataType;
        }

        public bool TryRead(out TData value)
        {
            return Factory.GetStorage(Context).TryRead(DataType.Attribute.Key, out value);
        }

        public void Write(TData value)
        {
            Factory.GetStorage(Context).Write(DataType.Attribute.Key, value);
        }
    }
}
