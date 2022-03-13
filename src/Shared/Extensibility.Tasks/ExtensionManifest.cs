using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class ExtensionManifest
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("codegen")]
        public ExtensionManifestCodegen? Codegen { get; set; }
    }

    public class ExtensionManifestCodegen
    {
        [JsonProperty("types")]
        public string[]? Types { get; set; }
    }
}
