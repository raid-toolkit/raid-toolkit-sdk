namespace Raid.DataServices
{
    public interface IDataReader
    {
        bool TryRead<T>(string key, out T value) where T : class;
    }
}
