using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension
{
    public class DataSpec<T> where T : class
    {
        private readonly string Key;
        public DataSpec(string key)
        {
            Key = key;
        }
        public T Get(IExtensionStorage storage)
        {
            if (!storage.TryRead<T>(Key, out T Value))
                throw new System.NullReferenceException("Could not obtain value");

            return Value;
        }
        public void Set(IExtensionStorage storage, T value)
        {
            storage.Write<T>(Key, value);
        }
    }
}
