using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension.Account
{
    internal class DataSpec<T> where T : class
    {
        private T Value;
        private string Key;
        public DataSpec(string key)
        {
            Key = key;
        }
        public T Get(CachedDataStorage<PersistedDataStorage> storage, IDataContext context)
        {
            if (Value == null)
                storage.TryRead<T>(context, Key, out Value);
            return Value ?? throw new System.NullReferenceException("Could not obtain value");
        }
    }
}
