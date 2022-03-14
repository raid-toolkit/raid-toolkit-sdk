using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticDataWrapper
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticDataWrapper(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        private static readonly DataSpec<StaticArenaData> _Arena = new("arena");
        public StaticArenaData Arena => _Arena.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticAcademyData> _Academy = new("academy");
        public StaticAcademyData Academy => _Academy.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticArtifactData> _Artifacts = new("artifacts");
        public StaticArtifactData Artifacts => _Artifacts.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticHeroTypeData> _HeroTypes = new("heroTypes");
        public StaticHeroTypeData HeroTypes => _HeroTypes.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticSkillData> _Skills = new("skills");
        public StaticSkillData Skills => _Skills.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticStageData> _Stages = new("stages");
        public StaticStageData Stages => _Stages.Get(Storage, StaticDataContext.Default);

        private static readonly DataSpec<StaticLocalizationData> _Localization = new("localization");
        public StaticLocalizationData Localization => _Localization.Get(Storage, StaticDataContext.Default);
    }
}