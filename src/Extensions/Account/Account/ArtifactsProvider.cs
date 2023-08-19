using System;
using System.Collections.Generic;
using System.Linq;

using Client.Model.Gameplay.Artifacts;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extension.Account
{
    public class ArtifactsDataObject : Dictionary<int, Artifact>
    {
    }

    public class ArtifactsProvider : DataProvider<AccountDataContext, ArtifactsDataObject>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "artifacts";
        public override Version Version => kVersion;

        private const string StateKey = "artifacts-state";
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly CachedDataStorage StateResolver;
        private readonly ILogger<ArtifactsProvider> Logger;

        public ArtifactsProvider(CachedDataStorage<PersistedDataStorage> storage, CachedDataStorage stateResolver, ILogger<ArtifactsProvider> logger)
        {
            Storage = storage;
            StateResolver = stateResolver;
            Logger = logger;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context, SerializedDataInfo info)
        {
            ModelScope scope = new(runtime);
            var artifactData = scope.AppModel._userWrapper.Artifacts.ArtifactData;

            IReadOnlyList<Artifact> artifacts;

            _ = Storage.TryRead(context, Key, out ArtifactsDataObject previous);
            if (!StateResolver.TryRead(context, StateKey, out ArtifactsProviderState state))
            {
                state = new ArtifactsProviderState();
            }
            bool hasUpdates = true;

            if (state.ShouldForceUpdate() || previous == null)
            {
                Logger.LogInformation("Performing full artifact update");
                artifacts = GetArtifacts(scope);

                // TODO: Defer this until the end of update, so we will retry a full reload if the current read throws.
                state.MarkRefresh(artifactData);
                _ = StateResolver.Write(context, StateKey, state); // kinda unnecessary, but semantically we should still commit the new value
            }
            else if (previous != null && state.ShouldIncrementalUpdate(artifactData))
            {
                Logger.LogInformation("Performing incremental artifact update");
                artifacts = previous.Values.ToList();
                hasUpdates = false;
            }
            else
            {
                Logger.LogInformation("Skipping artifact update");
                // no updates
                return false;
            }

            ArtifactsDataObject result = new();

            var updatedArtifacts = artifactData.UpdatedArtifacts;
            var deletedArtifacts = artifactData.DeletedArtifactIds;

            foreach (var artifactEntry in artifacts)
            {
                if (artifactEntry == null) continue;
                if (deletedArtifacts.Contains(artifactEntry.Id))
                {
                    hasUpdates = true;
                    continue;
                }

                if (updatedArtifacts.TryGetValue(artifactEntry.Id, out var artifact))
                {
                    result.Add(artifactEntry.Id, artifact.ToModel());
                    hasUpdates = true;
                }
                else
                {
                    result.Add(artifactEntry.Id, artifactEntry);
                }

            }

            if (hasUpdates)
            {
                return Storage.Write(context, Key, result);
            }
            return false;
        }

        private static IReadOnlyList<Artifact> GetArtifacts(ModelScope scope)
        {
            Client.Model.Guard.UserWrapper userWrapper = scope.AppModel._userWrapper;
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
            {
                ExternalArtifactsStorage storage = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver._implementation.GetValue(scope.Context) as ExternalArtifactsStorage;
                return storage._state._artifacts.Values.Select(ModelExtensions.ToModel).ToList();
            }
            return userWrapper.Artifacts.ArtifactData.Artifacts.Select(ModelExtensions.ToModel).ToList();
        }
    }
}
