using System.Linq;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticDataWrapper
    {
        private IAccount? Account;
        private readonly IExtensionHost? Host;
        public StaticDataWrapper(IExtensionHost host)
        {
            Host = host;
        }
        public StaticDataWrapper(IAccount account)
        {
            Account = account;
        }

        private IAccount CurrentAccount => Account ??= Host?.GetAccounts().FirstOrDefault()
                ?? throw new System.NullReferenceException("Static data not yet extracted. Start the game and allow extraction of an account to access this data.");


        private static readonly AccountDataSpec<StaticArenaData> _Arena = new();
        public StaticArenaData Arena => _Arena.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticAcademyData> _Academy = new();
        public StaticAcademyData Academy => _Academy.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticArtifactData> _Artifacts = new();
        public StaticArtifactData Artifacts => _Artifacts.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticHeroTypeData> _HeroTypes = new();
        public StaticHeroTypeData HeroTypes => _HeroTypes.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticSkillData> _Skills = new();
        public StaticSkillData Skills => _Skills.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticStageData> _Stages = new();
        public StaticStageData Stages => _Stages.Get(CurrentAccount);

        private static readonly AccountDataSpec<StaticLocalizationData> _Localization = new();
        public StaticLocalizationData Localization => _Localization.Get(CurrentAccount);
    }
}
