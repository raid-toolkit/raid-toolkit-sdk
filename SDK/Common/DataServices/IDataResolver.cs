using System;

namespace Raid.DataServices
{
    public interface IDataResolver<TContext, TData>
        where TContext : class, IDataContext
        where TData : class
    {
        IDataType<TData> DataType { get; }
        bool TryReadAs<T>(TContext context, out T value) where T : class;
        bool TryRead(TContext context, out TData value);
        bool Write(TContext context, TData value);
    }
    public interface IDataResolver<TContext, TStorage, TData> : IDataResolver<TContext, TData>
        where TContext : class, IDataContext
        where TStorage : class, IDataStorage, new()
        where TData : class
    {
    }

    public static class DataResolverExtensions
    {
        public static bool Update<TContext, TStorage, TData>(this IDataResolver<TContext, TStorage, TData> resolver, TContext context, Func<TData, TData> updateFn)
            where TContext : class, IDataContext
            where TStorage : class, IDataStorage, new()
            where TData : class
        {
            _ = resolver.TryRead(context, out TData value);
            return resolver.Write(context, updateFn(value));
        }
    }
    public class DataResolverManager<TContext, TStorage, TData> : IDataResolver<TContext, TStorage, TData>
        where TContext : class, IDataContext
        where TStorage : class, IDataStorage, new()
        where TData : class
    {
        private readonly IDataStorageFactory<TStorage> Factory;
        public IDataType<TData> DataType { get; }

        public DataResolverManager(IDataStorageFactory<TStorage> factory, IDataType<TData> dataType)
        {
            Factory = factory;
            DataType = dataType;
        }

        public bool TryRead(TContext context, out TData value)
        {
            return Factory.GetStorage(context).TryRead(DataType.Attribute.Key, out value);
        }

        public bool Write(TContext context, TData value)
        {
            return Factory.GetStorage(context).Write(DataType.Attribute.Key, value);
        }

        public bool TryReadAs<T>(TContext context, out T value) where T : class
        {
            return Factory.GetStorage(context).TryRead(DataType.Attribute.Key, out value);
        }
    }
}
