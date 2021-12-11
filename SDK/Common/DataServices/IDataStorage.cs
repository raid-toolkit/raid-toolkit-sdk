namespace Raid.DataServices
{
    public interface IDataStorage : IDataReader
    {
        void Write<T>(string key, T value);
    }
}
