using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public abstract class AccountDataProviderBase<T> : DataObjectProviderBase<T> where T : class
    {
        public AccountDataProviderBase(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, T> storage)
            : base(storage)
        {
        }

        public override T Update(T previousValue = null)
        {
            throw new System.NotImplementedException();
        }

        protected abstract T Update(ModelScope scope, T previousValue = null);
    }
}
