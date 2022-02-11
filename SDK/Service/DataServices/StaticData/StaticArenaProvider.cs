using System.Linq;
using Raid.DataModel;
using Raid.DataServices;

namespace Raid.Service.DataServices
{
    [DataType("arena", Version = "1.1")]
    public class StaticArenaDataObject : StaticArenaData
    {
    }

    public class StaticArenaProvider : DataProviderBase<StaticDataContext, StaticArenaDataObject>
    {
        public StaticArenaProvider(IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticArenaDataObject> storage)
            : base(storage)
        {
        }

        public override bool Update(ModelScope scope, StaticDataContext context)
        {
            var hash = scope.StaticDataManager._hash;
            if (PrimaryProvider.TryRead(context, out StaticArenaDataObject previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var arenaLeagues = staticData.ArenaData.LeagueInfoById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return PrimaryProvider.Write(context, new()
            {
                Hash = hash,
                Leagues = arenaLeagues.ToModel()
            });
        }
    }
}
