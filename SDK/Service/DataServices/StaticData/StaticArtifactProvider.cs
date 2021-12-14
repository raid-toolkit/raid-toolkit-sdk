using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("artifacts")]
    public class StaticArtifactDataObject : StaticArtifactData
    {
    }

    public class StaticArtifactProvider : DataProviderBase<StaticDataContext, StaticArtifactDataObject>
    {
        public StaticArtifactProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticArtifactDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticArtifactDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var artifactTypes = staticData.ArtifactData._setInfoByKind.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                ArtifactSetKinds = artifactTypes.ToModel()
            });
        }
    }
}
