using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension.Account
{
    public class AccountData
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly AccountDataContext Context;
        public AccountData(CachedDataStorage<PersistedDataStorage> storage, string accountId)
        {
            Storage = storage;
            Context = accountId;
        }

        private static readonly DataSpec<AccountBase> _Account = new("account");
        public AccountBase Account => _Account.Get(Storage, Context);

        private static readonly DataSpec<ArenaData> _Arena = new("arena");
        public ArenaData Arena => _Arena.Get(Storage, Context);

        private static readonly DataSpec<AcademyData> _Academy = new("academy");
        public AcademyData Academy => _Academy.Get(Storage, Context);

        private static readonly DataSpec<ArtifactsDataObject> _Artifacts = new("artifacts");
        public ArtifactsDataObject Artifacts => _Artifacts.Get(Storage, Context);

        private static readonly DataSpec<HeroData> _HeroData = new("heroes");
        public HeroData Heroes => _HeroData.Get(Storage, Context);

        private static readonly DataSpec<Resources> _Resources = new("resources");
        public Resources Resources => _Resources.Get(Storage, Context);
    }
}