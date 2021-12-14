using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("skills")]
    public class StaticSkillDataObject : StaticSkillData
    {
    }

    public class StaticSkillProvider : DataProviderBase<StaticDataContext, StaticSkillDataObject>
    {
        public StaticSkillProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticSkillDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticSkillDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var skillTypes = staticData.SkillData._skillTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                SkillTypes = skillTypes
            });
        }
    }
}
