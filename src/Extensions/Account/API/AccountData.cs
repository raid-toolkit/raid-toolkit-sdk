using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension.Account
{
    public interface IAccountData
    {
        AccountBase Account { get; }
        ArenaData Arena { get; }
        AcademyData Academy { get; }
        ArtifactsDataObject Artifacts { get; }
        HeroData Heroes { get; }
        Resources Resources { get; }
    }
    public class ImportedAccountData : IAccountData
    {
        public AccountBase Account { get; set; }
        public ArenaData Arena { get; set; }
        public AcademyData Academy { get; set; }
        public ArtifactsDataObject Artifacts { get; set; }
        public HeroData Heroes { get; set; }
        public Resources Resources { get; set; }
    }
    public class AccountData : IAccountData
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly AccountDataContext Context;
        public AccountData(CachedDataStorage<PersistedDataStorage> storage, string accountId)
        {
            Storage = storage;
            Context = accountId;
        }

        private static readonly DataSpec<AccountBase> _Account = new("account");
        public AccountBase Account
        {
            get => _Account.Get(Storage, Context);
            set => _Account.Set(Storage, Context, value);
        }

        private static readonly DataSpec<ArenaData> _Arena = new("arena");
        public ArenaData Arena
        {
            get => _Arena.Get(Storage, Context);
            set => _Arena.Set(Storage, Context, value);
        }

        private static readonly DataSpec<AcademyData> _Academy = new("academy");
        public AcademyData Academy
        {
            get => _Academy.Get(Storage, Context);
            set => _Academy.Set(Storage, Context, value);
        }

        private static readonly DataSpec<ArtifactsDataObject> _Artifacts = new("artifacts");
        public ArtifactsDataObject Artifacts
        {
            get => _Artifacts.Get(Storage, Context);
            set => _Artifacts.Set(Storage, Context, value);
        }

        private static readonly DataSpec<HeroData> _HeroData = new("heroes");
        public HeroData Heroes
        {
            get => _HeroData.Get(Storage, Context);
            set => _HeroData.Set(Storage, Context, value);
        }

        private static readonly DataSpec<Resources> _Resources = new("resources");
        public Resources Resources
        {
            get => _Resources.Get(Storage, Context);
            set => _Resources.Set(Storage, Context, value);
        }
    }
}
