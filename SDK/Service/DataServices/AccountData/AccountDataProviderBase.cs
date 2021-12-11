using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public interface IAccountDataProvider
    {
        object Update(ModelScope scope, AccountDataContext context);
    }

    public abstract class AccountDataProviderBase<T> : DataObjectProviderBase<AccountDataContext, T>, IAccountDataProvider where T : class
    {
        public AccountDataProviderBase(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, T> storage)
            : base(storage)
        {
        }

        public abstract T Update(ModelScope scope, AccountDataContext context);

        object IAccountDataProvider.Update(ModelScope scope, AccountDataContext context)
        {
            return Update(scope, context);
        }
    }
}
