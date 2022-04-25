using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ExtensionStorage : IExtensionStorage
    {
        private readonly IDataStorage Storage;
        private readonly IDataContext Context;

        public ExtensionStorage(IDataStorage storage, ExtensionDataContext context)
        {
            Storage = storage;
            Context = context;
        }

        public bool TryRead<T>(string key, out T value) where T : class
        {
            return Storage.TryRead<T>(Context, key, out value);
        }

        public void Write<T>(string key, T value) where T : class
        {
            Storage.Write<T>(Context, key, value);
        }
    }
}
