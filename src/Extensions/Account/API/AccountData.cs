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
        private readonly IAccount Owner;
        public AccountData(IAccount account)
        {
            Owner = account;
        }

        public AccountBase Account => Owner.AccountInfo;

        private static readonly AccountDataSpec<ArenaData> _Arena = new();
        public ArenaData Arena => _Arena.Get(Owner);

        private static readonly AccountDataSpec<AcademyData> _Academy = new();
        public AcademyData Academy => _Academy.Get(Owner);

        private static readonly AccountDataSpec<ArtifactsDataObject> _Artifacts = new();
        public ArtifactsDataObject Artifacts => _Artifacts.Get(Owner);

        private static readonly AccountDataSpec<HeroData> _HeroData = new();
        public HeroData Heroes => _HeroData.Get(Owner);

        private static readonly AccountDataSpec<Resources> _Resources = new();
        public Resources Resources => _Resources.Get(Owner);
    }
}
