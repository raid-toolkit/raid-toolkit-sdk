using System;

using Newtonsoft.Json;

namespace Raid.Toolkit.Extension.Account
{
    public class ArtifactsProviderState
    {
        private const int kForceRefreshInterval = 60000;
        [JsonProperty("nextForcedRefresh")]
        private DateTime m_nextForcedRefresh = DateTime.MinValue;
        [JsonProperty("nextId")]
        private int m_nextId;
        [JsonProperty("nextRevisionId")]
        private int m_nextRevisionId;

        public bool ShouldForceUpdate()
        {
            return DateTime.UtcNow > m_nextForcedRefresh;
        }
        public bool ShouldIncrementalUpdate(SharedModel.Meta.Artifacts.UserArtifactData artifactData)
        {
            // Only refresh if m_nextRevisionId changed since last read, or after we've exceeded the forced read interval
            return artifactData.NextArtifactId != m_nextId
                || artifactData.NextArtifactRevisionId != m_nextRevisionId;
        }
        public void MarkRefresh(SharedModel.Meta.Artifacts.UserArtifactData artifactData)
        {
            if (ShouldForceUpdate()) // if we're overtime
                m_nextForcedRefresh = DateTime.UtcNow.AddMilliseconds(kForceRefreshInterval);
            m_nextId = artifactData.NextArtifactId;
            m_nextRevisionId = artifactData.NextArtifactRevisionId;
        }
    }
}
