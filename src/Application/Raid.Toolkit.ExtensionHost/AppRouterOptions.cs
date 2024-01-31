using CommandLine;

namespace Raid.Toolkit.ExtensionHost;

public abstract class BaseOptions
{
	public abstract string GetPackageId();

	[Option('q', "quiet")]
	public bool DisableLogging { get; set; }

	[Option("Embedding")]
	public bool Embedding { get; set; }

	[Option("force-rebuild")]
	public bool ForceRebuild { get; set; }

	[Option('d', "debug", Hidden = true)]
	public bool Debug { get; set; }

	[Option('s', "standalone", Hidden = true)]
	public bool Standalone { get; set; }

	[Option('p', "debug-package", Hidden = true)]
	public string? DebugPackage { get; set; }

	[Option("dbg-break", Hidden = true)]
	public bool DebugBreak { get; set; }
}

[Verb("install", HelpText = "Installs an extension package")]
class InstallPackageOptions : BaseOptions
{
	[Value(0, MetaName = "rtkx", HelpText = "Path to RTKX package to install")]
	public string PackagePath { get; set; } = string.Empty;

	[Option('a', "accept", Hidden = true)]
	public bool Accept { get; set; } = false;

	private ExtensionBundle? _bundle;
	public ExtensionBundle Bundle => _bundle ??= ExtensionBundle.FromFile(PackagePath);

	public override string GetPackageId()
	{
		return Bundle.Manifest.Id;
	}
}

[Verb("run", HelpText = "Runs an installed extension package")]
class RunPackageOptions : BaseOptions
{
	[Value(0, MetaName = "Package id", HelpText = "Id of the extension package to run")]
	public string PackageId { get; set; } = string.Empty;

	public override string GetPackageId()
	{
		return PackageId;
	}
}

[Verb("activate", HelpText = "Invokes an extension's activation protocol handler")]
class ActivateExtensionOptions : BaseOptions
{
	[Value(0, MetaName = "uri", HelpText = "Activation uri")]
	public string Uri { get; set; } = string.Empty;

	[Value(1, MetaName = "Arguments", HelpText = "Additional arguments")]
	public IEnumerable<string> Arguments { get; set; } = Array.Empty<string>();


	public override string GetPackageId()
	{
		Uri activationUri = new(Uri);
		if (activationUri.Host != "extension")
			throw new ArgumentOutOfRangeException(nameof(activationUri));
		string packageId = activationUri.LocalPath.TrimStart('/').Split('/')[0];
		return packageId;
	}
}
