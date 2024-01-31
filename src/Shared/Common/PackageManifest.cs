using System.ComponentModel;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility;

public class PackageManifest
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public PackageManifest()
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

	[JsonProperty("compatibleVersion")]
	public string? CompatibleVersionString { get; set; }

	[JsonIgnore]
	public Version CompatibleVersion { get => Version.Parse(CompatibleVersionString ?? RequireVersion ?? "2.0.0"); set => CompatibleVersionString = value.ToString(); }

	[JsonProperty("contributes")]
	public Contributes? Contributes;

	public static PackageManifest FromAssembly(Assembly asm)
	{
		JsonSerializer serializer = new();
		using Stream stream = asm.GetManifestResourceStream("PackageManifest") ?? throw new FileNotFoundException("PackageManifest");
		using StreamReader reader = new(stream);
		using JsonTextReader textReader = new(reader);
		{
			return serializer.Deserialize<PackageManifest>(textReader)!;
		}
	}
}

public class Contributes
{
	public List<MenuContribution>? MenuItems { get; set; }
}

public class MenuContribution
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Exists for serialization only")]
	public MenuContribution()
	{
		Name = string.Empty;
		DisplayName = string.Empty;
	}

	public MenuContribution(string name, string displayName)
	{
		Name = name;
		DisplayName = displayName;
	}

	[JsonProperty("name")]
	public string Name { get; }

	[JsonProperty("displayName")]
	public string DisplayName { get; }
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
