using System;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Il2CppToolkit.Runtime;
using System.Collections.Generic;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticLocalizationProvider : DataProvider<StaticDataContext, StaticLocalizationData>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "localization";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        public StaticLocalizationProvider(CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context)
        {
            ModelScope scope = new(runtime);
            var hash = scope.StaticDataManager._hash;
            if (Storage.TryRead(context, Key, out StaticLocalizationData previous))
            {
                if (previous?.Hash == hash)
                    return false;
            }
            var staticData = scope.StaticDataManager.StaticData;
            var localizedStrings = new Dictionary<string, string>();
            foreach(var entry in staticData.ClientLocalization.Concat(staticData.StaticDataLocalization)
                .GroupBy(x => x.Key)
                .Select(g => g.First()))
            {
                localizedStrings.Add(entry.Key, entry.Value);
            }
            return Storage.Write(context, Key, new StaticLocalizationData
            {
                Hash = hash,
                LocalizedStrings = localizedStrings
            });
        }
    }
}
