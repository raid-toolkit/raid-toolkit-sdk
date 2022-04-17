using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppToolkit.Runtime;
using Newtonsoft.Json;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Providers;

namespace Raid.Toolkit.Extension.Account
{
    public class ArtifactsDataObject : Dictionary<int, Artifact>
    {
    }

    public class ArtifactsProviderState
    {
        private const int kForceRefreshInterval = 60000;
        [JsonProperty("nextForcedRefresh")]
        private DateTime m_nextForcedRefresh = DateTime.MinValue;
        [JsonProperty("nextId")]
        private int m_nextId;
        [JsonProperty("nextRevisionId")]
        private int m_nextRevisionId;

        public bool ShouldIncrementalUpdate(SharedModel.Meta.Artifacts.UserArtifactData artifactData)
        {
            // Only refresh if m_nextRevisionId changed since last read, or after we've exceeded the forced read interval
            return DateTime.UtcNow >= m_nextForcedRefresh
                || artifactData.NextArtifactId != m_nextId
                || artifactData.NextArtifactRevisionId != m_nextRevisionId;
        }
        public void MarkRefresh(SharedModel.Meta.Artifacts.UserArtifactData artifactData)
        {
            m_nextForcedRefresh = DateTime.UtcNow.AddMilliseconds(kForceRefreshInterval);
            m_nextId = artifactData.NextArtifactId;
            m_nextRevisionId = artifactData.NextArtifactRevisionId;
        }
    }

    public class ArtifactsProvider : DataProvider<AccountDataContext, ArtifactsDataObject>
    {
        private static Version kVersion = new(2, 0);

        public override string Key => "artifacts";
        public override Version Version => kVersion;

        private const string StateKey = "artifacts-state";
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly CachedDataStorage StateResolver;

        public ArtifactsProvider(CachedDataStorage<PersistedDataStorage> storage, CachedDataStorage stateResolver)
        {
            Storage = storage;
            StateResolver = stateResolver;
        }

        public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
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

            if (previous != null && state.ShouldIncrementalUpdate(artifactData))
            {
                artifacts = previous.Values.ToList();
                hasUpdates = false;
            }
            else
            {
                artifacts = GetArtifacts(scope);

                // TODO: Defer this until the end of update, so we will retry a full reload if the current read throws.
                state.MarkRefresh(artifactData);
                _ = StateResolver.Write(context, StateKey, state); // kinda unnecessary, but semantically we should still commit the new value
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
            var artifactStorageResolver = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver.GetInstance(scope.Context);
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
            {
                var storage = artifactStorageResolver._implementation as Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage;
                return storage._state._artifacts.Values.Select(ModelExtensions.ToModel).ToList();
            }
            return userWrapper.Artifacts.ArtifactData.Artifacts.Select(ModelExtensions.ToModel).ToList();
        }
    }
}
