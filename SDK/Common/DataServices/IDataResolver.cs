namespace Raid.DataServices
{
    public interface IDataResolver<C, T, D>
        where C : IDataContext
        where T : IDataStorage
        where D : class
    {
        bool TryRead(out D value);
        void Write(D value);
    }

    public class DataResolverManager<C, T, D> : IDataResolver<C, T, D>
        where C : IDataContext
        where T : IDataStorage
        where D : class
    {
        private readonly IDataContext Context;
        private readonly IDataStorageFactory<T> Factory;
        private readonly IDataType<D> DataType;
        public DataResolverManager(C context, IDataStorageFactory<T> factory, IDataType<D> dataType)
        {
            Context = context;
            Factory = factory;
            DataType = dataType;
        }
        public bool TryRead(out D value)
        {
            return Factory.GetStorage(Context).TryRead(DataType.Attribute.Key, out value);
        }

        public void Write(D value)
        {
            Factory.GetStorage(Context).Write(DataType.Attribute.Key, value);
        }
    }
}
