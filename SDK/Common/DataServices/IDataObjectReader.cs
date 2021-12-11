namespace Raid.DataServices
{
    public interface IDataObjectReader<T> where T : class
    {
        T Update(T previousValue = null);
    }

    public class DataObjectReaderBase<T> : IDataObjectReader<T> where T : class
    {
        public virtual T Update(T previousValue = null)
        {
            return previousValue;
        }
    }
}