namespace Raid.Service
{
    public interface IModelDataSource
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T value) where T : class;
    }
}
