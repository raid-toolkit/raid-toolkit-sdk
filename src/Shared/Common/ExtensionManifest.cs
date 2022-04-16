using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility
{
    public class ExtensionManifest
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("asm")]
        public string Assembly { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("codegen")]
        public ExtensionManifestCodegen Codegen { get; set; }

        public static ExtensionManifest FromAssembly(Assembly asm)
        {
            JsonSerializer serializer = new();
            using (var stream = asm.GetManifestResourceStream("PackageManifest"))
            using (StreamReader reader = new(stream))
            using (JsonTextReader textReader = new(reader))
            {
                return serializer.Deserialize<ExtensionManifest>(textReader);
            }
        }
    }

    public class ExtensionManifestCodegen
    {
        [JsonProperty("types")]
        public string[] Types { get; set; }
    }
}
