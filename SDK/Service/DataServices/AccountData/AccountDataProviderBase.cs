using System;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public interface IAccountDataProvider
    {
        DataTypeAttribute DataType { get; }
        bool Upgrade(AccountDataContext context, Version dataVersion);
        bool Update(ModelScope scope, AccountDataContext context);
    }

    public abstract class AccountDataProviderBase<T> : DataObjectProviderBase<AccountDataContext, T>, IAccountDataProvider where T : class
    {
        public AccountDataProviderBase(IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, T> storage)
            : base(storage)
        {
        }

        public virtual bool Upgrade(AccountDataContext context, Version dataVersion)
        {
            return false;
        }

        public abstract bool Update(ModelScope scope, AccountDataContext context);
    }
}
