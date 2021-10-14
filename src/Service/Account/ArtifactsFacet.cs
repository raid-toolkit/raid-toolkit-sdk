using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service.DataModel;

namespace Raid.Service
{
    [Facet("artifacts")]
    public class ArtifactsFacet : Facet<IReadOnlyDictionary<int, Artifact>, ArtifactsFacet>
    {
        private const int kForceRefreshInterval = 30000;
        private DateTime m_nextForcedRefresh = DateTime.MinValue;
        private int m_nextId;
        private int m_nextRevisionId;

        protected override IReadOnlyDictionary<int, Artifact> Merge(ModelScope scope, IReadOnlyDictionary<int, Artifact> previous = null)
        {
            var artifactData = scope.AppModel._userWrapper.Artifacts.ArtifactData;

            // Only refresh if lastHeroId changed since last read, or after we've exceeded the forced read interval
            if (DateTime.UtcNow < m_nextForcedRefresh
                && artifactData.NextArtifactId == m_nextId
                && artifactData.NextArtifactRevisionId == m_nextRevisionId)
            {
                return previous;
            }
            m_nextForcedRefresh = DateTime.UtcNow.AddMilliseconds(kForceRefreshInterval);
            m_nextId = artifactData.NextArtifactId;
            m_nextRevisionId = artifactData.NextArtifactRevisionId;

            Dictionary<int, Artifact> result = new();
            var artifacts = GetArtifacts(scope);

            var updatedArtifacts = artifactData.UpdatedArtifacts;
            var deletedArtifacts = artifactData.DeletedArtifactIds;

            foreach (var artifactEntry in artifacts)
            {
                if (artifactEntry == null) continue;
                if (deletedArtifacts.Contains(artifactEntry._id)) continue;

                if (!updatedArtifacts.TryGetValue(artifactEntry._id, out var artifact))
                {
                    artifact = artifactEntry;
                }

                result.Add(artifact._id, new Artifact()
                {
                    Id = artifact._id,
                    KindId = artifact._kindId,
                    SetKindId = artifact._setKindId,
                    Level = artifact._level,
                    Rank = artifact._rankId,
                    RarityId = artifact._rarityId,
                    Seen = artifact._isSeen,
                    Activated = artifact._isActivated,
                    SellPrice = artifact._sellPrice,
                    Price = artifact._price,
                    Faction = artifact._requiredFraction,
                    FailedUpgrades = artifact._failedUpgrades,
                    Revision = artifact._revision,
                    PrimaryBonus = artifact._primaryBonus.ToDataModel(),
                    SecondaryBonuses = artifact._secondaryBonuses.Select(ModelConverters.ToDataModel).ToList()
                });
            }
            return result;
        }

        private static IReadOnlyList<SharedModel.Meta.Artifacts.Artifact> GetArtifacts(ModelScope scope)
        {
            Client.Model.Guard.UserWrapper userWrapper = scope.AppModel._userWrapper;
            var artifactStorageResolver = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver.GetInstance(scope.Context);
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
            {
                var storage = artifactStorageResolver._implementation as Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage;
                return storage._state._artifacts.Values.ToList();
            }
            return userWrapper.Artifacts.ArtifactData.Artifacts;
        }
    }
}