namespace Raid.DataServices
{
    public interface IDataObjectProvider
    {
        object GetValue();
        object Update(object previousValue = null);
    }

    public interface IDataObjectProvider<T> : IDataObjectProvider where T : class
    {
        new T GetValue();
        T Update(T previousValue = null);
    }

    public class DataObjectProviderBase<T> : IDataObjectProvider<T> where T : class
    {
        private readonly IDataResolver<T> PrimaryProvider;
        public DataObjectProviderBase(IDataResolver<T> primaryProvider)
        {
            PrimaryProvider = primaryProvider;
        }
        public virtual T Update(T previousValue = null)
        {
            return previousValue;
        }

        public virtual T GetValue()
        {
            return PrimaryProvider.TryRead(out T value) ? value : default;
        }

        object IDataObjectProvider.GetValue()
        {
            return GetValue();
        }

        object IDataObjectProvider.Update(object previousValue)
        {
            return Update((T)previousValue);
        }
    }
}