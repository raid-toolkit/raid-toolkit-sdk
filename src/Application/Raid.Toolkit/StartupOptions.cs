using CommandLine;

namespace Raid.Toolkit;

public class StartupOptions
{
	[Option('p', "package", HelpText = "Path to rtkx package to install", Hidden = true)]
	public string PackagePath { get; set; } = string.Empty;

	[Option('q', "quiet")]
	public bool DisableLogging { get; set; }

	[Option("Embedding")]
	public bool Embedding { get; set; }

	[Option('d', "debug", Hidden = true)]
	public bool Debug { get; set; }

	[Option("dbg-break", Hidden = true)]
	public bool DebugBreak { get; set; }

	[Option('s', "standalone", Hidden = true)]
	public bool Standalone { get; set; }

	[Option('p', "debug-package", Hidden = true)]
	public string? DebugPackage { get; set; }

	[Option('w', "wait", HelpText = "Wait <ms> for an existing instance to shut down before starting")]
	public int? Wait { get; set; }

	[Option('u', "post-update")]
	public bool Update { get; set; }

	[Option(longName: "--AppNotificationActivated")]
	public bool AppNotificationActivated { get; set; }

	[Option("nologo")]
	public bool NoLogo { get; set; }

	[Option("force-rebuild")]
	public bool ForceRebuild { get; set; }
}
