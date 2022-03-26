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
        public T Get(CachedDataStorage<PersistedDataStorage> storage, IDataContext context)
        {
            if (!storage.TryRead<T>(context, Key, out T Value))
                throw new System.NullReferenceException("Could not obtain value");

            return Value;
        }
    }
}
