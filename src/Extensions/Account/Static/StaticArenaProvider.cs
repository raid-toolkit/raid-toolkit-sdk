using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticArenaProvider : DataProvider<StaticDataContext, StaticArenaData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "arena";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticArenaProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            ModelScope scope = new(runtime);
            var hash = scope.StaticDataManager._hash;
            if (Storage.TryRead(context, Key, out StaticArenaData previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var arenaLeagues = staticData.ArenaData.LeagueInfoById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel());
            return Storage.Write(context, Key, new StaticArenaData
            {
                Hash = hash,
                Leagues = arenaLeagues.ToModel()
            });
        }
    }
}
