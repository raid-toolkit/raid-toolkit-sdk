namespace Raid.Toolkit.Extensibility.DataServices
{
    public interface IDataReader
    {
        bool TryRead<T>(IDataContext context, string key, out T value) where T : class;
    }
}
