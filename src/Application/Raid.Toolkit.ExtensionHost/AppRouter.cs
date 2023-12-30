using CommandLine;

using Microsoft.Win32.SafeHandles;
using Microsoft.Windows.AppLifecycle;

using Raid.Toolkit.Extensibility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;
using Windows.Win32.Security;

using WinRT;

using PInvoke = Windows.Win32.PInvoke;

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

	[Option('p', "debug-package", Hidden = true)]
	public string? DebugPackage { get; set; }

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
        string packageId = activationUri.LocalPath.TrimStart('/').Split('/')[0];
        return packageId;
    }
}

internal class AppRouter
{
    private static string GetInstanceKey(string extensionId)
    {
        return $"RAID_TOOLKIT_WORKER_{extensionId.ToUpperInvariant()}";
    }

    public AppInstance Instance { get; set; }
    public BaseOptions Options { get; }
    public event EventHandler<BaseOptions>? Activated;

    public AppRouter()
    {
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        Options = ParseActivationArguments(args);

        string instanceKey = GetInstanceKey(Options.GetPackageId());
        Instance = AppInstance.FindOrRegisterForKey(instanceKey);
        if (Instance.IsCurrent)
            Instance.Activated += OnActivated;
        else
            RedirectActivationTo(args, Instance);
    }

    private BaseOptions ParseActivationArguments(AppActivationArguments args)
    {
        return args.Kind switch
        {
            ExtendedActivationKind.Launch => ParseArguments(args.Data.As<ILaunchActivatedEventArgs>().Arguments),
            ExtendedActivationKind.CommandLineLaunch => ParseArguments(args.Data.As<ICommandLineActivatedEventArgs>().Operation.Arguments),
            ExtendedActivationKind.File => new InstallPackageOptions() { PackagePath = args.Data.As<IFileActivatedEventArgs>().Files[0].Path, },
            ExtendedActivationKind.Protocol => new ActivateExtensionOptions() { Uri = args.Data.As<IProtocolActivatedEventArgs>().Uri.ToString() },
            _ => throw new InvalidOperationException()
        };
    }

    private void OnActivated(object? sender, AppActivationArguments args)
    {
        Activated?.Invoke(this, ParseActivationArguments(args));
    }

    private static BaseOptions ParseArguments(string arguments)
    {
        string[] args = Regex.Matches(arguments, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(x => x.Value.Trim('"'))
                .Skip(1)
                .ToArray();
        BaseOptions? opts = null;
        Parser.Default.ParseArguments<InstallPackageOptions, RunPackageOptions, ActivateExtensionOptions>(args)
            .WithParsed<BaseOptions>(options => opts = options)
            .WithNotParsed(_ => throw new ArgumentException("Invalid arguments"));
        if (opts == null)
            throw new ArgumentException("Invalid arguments");
        return opts;
    }

    // Do the redirection on another thread, and use a non-blocking
    // wait method to wait for the redirection to complete.
    public static void RedirectActivationTo(
        AppActivationArguments args, AppInstance keyInstance)
    {
        using SafeFileHandle redirectEventHandle = PInvoke.CreateEvent((SECURITY_ATTRIBUTES?)null, true, false, null);
        Task.Run(() =>
        {
            keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
            PInvoke.SetEvent(redirectEventHandle);
        });
        uint CWMO_DEFAULT = 0;
        uint INFINITE = 0xFFFFFFFF;
        _ = PInvoke.CoWaitForMultipleObjects(CWMO_DEFAULT, INFINITE,
            new Windows.Win32.Foundation.HANDLE[] { new(redirectEventHandle.DangerousGetHandle()) },
            out uint handleIndex);
    }
}
