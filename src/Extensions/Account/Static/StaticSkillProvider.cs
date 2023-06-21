using System;
using System.Linq;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extension.Account
{
    public class StaticSkillProvider : DataProvider<StaticDataContext, StaticSkillData>
    {
        private static Version kVersion = new(2, 1);

        public override string Key => "skills";
        public override Version Version => kVersion;

        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly ILogger<StaticSkillProvider> Logger;
        public StaticSkillProvider(ILogger<StaticSkillProvider> logger, CachedDataStorage<PersistedDataStorage> storage)
        {
            Storage = storage;
        }

        public override bool Update(Il2CsRuntimeContext runtime, StaticDataContext context, SerializedDataInfo info)
        {
            ModelScope scope = new(runtime);
            var hash = scope.StaticDataManager._hash;
            if (Storage.TryRead(context, Key, out StaticSkillData? staticSkillData))
            {
                if (staticSkillData?.Hash == hash)
                    return false;
            }
            try
            {
                var staticData = scope.StaticDataManager.StaticData;
                staticSkillData = new()
                {
                    Hash = hash,
                    SkillTypes = staticData.SkillData._skillTypeById.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel())
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.StaticDataReadError.EventId(), ex, "Failed to read static data");
                return false;
            }
            return Storage.Write(context, Key, staticSkillData);
        }
    }
}
