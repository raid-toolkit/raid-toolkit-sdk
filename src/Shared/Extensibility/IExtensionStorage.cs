namespace Raid.Toolkit.Extensibility
{
    public interface IExtensionStorage
    {
        bool TryRead<T>(string key, out T value) where T : class;
        void Write<T>(string key, T value) where T : class;
    }
}
