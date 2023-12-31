using System.ComponentModel;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Raid.Toolkit.Extensibility;

public class ExtensionManifest
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public ExtensionManifest()
	{
		Id = string.Empty;
		Type = string.Empty;
		Assembly = string.Empty;
		DisplayName = string.Empty;
		Description = string.Empty;
		RequireVersion = string.Empty;
	}

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

	[JsonProperty("author")]
	public string? Author { get; set; }

	[JsonProperty("codegen")]
	public ExtensionManifestCodegen? Codegen { get; set; }

	[JsonProperty("requireVersion")]
	public string RequireVersion { get; set; }

	public static ExtensionManifest FromAssembly(Assembly asm)
	{
		JsonSerializer serializer = new();
		using Stream stream = asm.GetManifestResourceStream("PackageManifest") ?? throw new FileNotFoundException("PackageManifest");
		using StreamReader reader = new(stream);
		using JsonTextReader textReader = new(reader);
		{
			return serializer.Deserialize<ExtensionManifest>(textReader)!;
		}
	}
}

public class ExtensionManifestCodegen
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public ExtensionManifestCodegen()
	{
		Types = Array.Empty<string>();
	}

	[JsonProperty("types")]
	public string[] Types { get; set; }
}
