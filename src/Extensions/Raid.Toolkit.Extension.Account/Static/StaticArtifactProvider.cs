using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticArtifactProvider : DataProvider<StaticDataContext, StaticArtifactData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "artifacts";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticArtifactProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            ModelScope scope = new(runtime);
            var hash = scope.StaticDataManager._hash;
            if (Storage.TryRead(context, Key, out StaticArtifactData previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var artifactTypes = staticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return Storage.Write(context, Key, new StaticArtifactData
            {
                Hash = hash,
                ArtifactSetKinds = artifactTypes.ToModel()
            });
        }
    }
}
