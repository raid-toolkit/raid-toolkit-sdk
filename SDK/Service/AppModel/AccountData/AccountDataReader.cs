using Raid.DataServices;

namespace Raid.Service.DataServices
{
    public class AccountDataReaderState
    { }

    public class AccountDataReader : DataObjectReaderBase<AccountDataObject>
    {
        public AccountDataReader(
            IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, AccountDataObject> storage,
            IDataResolver<AccountDataContext, CachedDataStorage, AccountDataReaderState> state
            )
        {

        }

        public AccountDataObject Update(AccountDataObject previousValue = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
