using CommandLine;

using Microsoft.Win32.SafeHandles;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.Win32.Security;

using WinRT;

using PInvoke = Windows.Win32.PInvoke;

namespace Raid.Toolkit;

internal class AppRouter
{
	private const string InstanceKey = "RAID_TOOLKIT_EXE";

	public AppInstance Instance { get; set; }
	public StartupOptions Options { get; }
	public event EventHandler<StartupOptions>? Activated;

	public AppRouter()
	{
		AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
		Options = ParseActivationArguments(args);
		if (Options.DebugBreak)
		{
			while (!Debugger.IsAttached)
				Thread.Sleep(100);
			Debugger.Break();
		}


		Instance = AppInstance.FindOrRegisterForKey(InstanceKey);
		if (Instance.IsCurrent)
			Instance.Activated += OnActivated;
		else
			RedirectActivationTo(args, Instance);
	}

	private StartupOptions ParseActivationArguments(AppActivationArguments args)
	{
		return args.Kind switch
		{
			ExtendedActivationKind.Launch => ParseArguments(args.Data.As<ILaunchActivatedEventArgs>().Arguments),
			ExtendedActivationKind.CommandLineLaunch => ParseArguments(args.Data.As<ICommandLineActivatedEventArgs>().Operation.Arguments),
			ExtendedActivationKind.File => new StartupOptions() { PackagePath = args.Data.As<IFileActivatedEventArgs>().Files[0].Path, },
			ExtendedActivationKind.Protocol => throw new V3NotImpl(),
			_ => throw new InvalidOperationException()
		};
	}

	private void OnActivated(object? sender, AppActivationArguments args)
	{
		Activated?.Invoke(this, ParseActivationArguments(args));
	}

	private static StartupOptions ParseArguments(string arguments)
	{
		string[] args = Regex.Matches(arguments, @"[\""].+?[\""]|[^ ]+")
				.Cast<Match>()
				.Select(x => x.Value.Trim('"'))
				.Skip(1)
				.ToArray();
		StartupOptions? opts = null;
		Parser.Default.ParseArguments<StartupOptions>(args)
			.WithParsed(options => opts = options)
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
