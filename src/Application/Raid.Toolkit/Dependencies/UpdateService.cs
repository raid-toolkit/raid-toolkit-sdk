using System.Diagnostics;
using System.Reflection;
using System.Threading;

using CommunityToolkit.WinUI.Notifications;

using GitHub;
using GitHub.Schema;

using Il2CppToolkit.Common.Errors;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Extensibility.Notifications;
using Raid.Toolkit.Model;

namespace Raid.Toolkit;

public class UpdateSettings
{
	public int PollIntervalMs { get; set; }
}
public class UpdateService : PollingBackgroundService, IUpdateService
{
	public bool IsEnabled => true;

	private readonly Updater Updater;
	private readonly IApplicationModel ApplicationModel;
	private readonly INotificationSink Notify;
	private readonly bool Enabled;
	private readonly Version CurrentVersion;
	private Release? PendingRelease;
	private readonly TimeSpan _PollInterval;
	private protected override TimeSpan PollInterval => _PollInterval;

	public event EventHandler<UpdateAvailableEventArgs>? UpdateAvailable;

	public UpdateService(
		ILogger<UpdateService> logger,
		IOptions<UpdateSettings> settings,
		IApplicationModel applicationModel,
		Updater updater,
		INotificationManager notificationManager)
		: base(logger)
	{
		string? fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).FileVersion;
		ErrorHandler.VerifyElseThrow(fileVersion != null, ServiceError.UnhandledException, "Missing version information");
		CurrentVersion = Version.Parse(fileVersion!);
		Updater = updater;
		ApplicationModel = applicationModel;
		Enabled = RegistrySettings.AutomaticallyCheckForUpdates;
		_PollInterval = settings.Value.PollIntervalMs > 0 ? TimeSpan.FromMilliseconds(settings.Value.PollIntervalMs) : new TimeSpan(0, 15, 0);

		Notify = new NotificationSink("update");
		Notify.Activated += Notify_Activated;
		notificationManager.RegisterHandler(Notify);
	}

	private async void Notify_Activated(object? sender, NotificationActivationEventArgs e)
	{
		if (e.Arguments.TryGetValue(NotificationConstants.Action, out string? action))
		{
			switch (action)
			{
				case "install-update":
					{
						await InstallUpdate();
						break;
					}

				default:
					break;
			}
		}
	}

	public async Task InstallUpdate()
	{
		PendingRelease ??= await Updater.GetLatestRelease();
		if (PendingRelease != null)
			await InstallRelease(PendingRelease);
	}

	public async Task InstallRelease(Release release)
	{
		try
		{
			Stream newRelease = await Updater.DownloadSetup(release, null);
			string tempDownload = Path.Combine(Path.GetTempPath(), $"RaidToolkitSetup.exe");
			if (File.Exists(tempDownload))
				File.Delete(tempDownload);

			using (Stream newFile = File.Create(tempDownload))
			{
				_ = newRelease.Seek(0, SeekOrigin.Begin);
				newRelease.CopyTo(newFile);
			}

			_ = Process.Start(tempDownload, "/update LaunchAfterInstallation=1");
			ApplicationModel.Exit();
		}
		catch (Exception ex)
		{
			Logger.LogError(ServiceError.MissingUpdateAsset.EventId(), ex, "Failed to update to {tagName}", release.TagName);
			throw;
		}
	}

	protected override async Task ExecuteOnceAsync(CancellationToken token)
	{
		try
		{
			if (Enabled)
			{
				_ = await CheckForUpdates(false, false);
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ServiceError.UnhandledException.EventId(), ex, "Unhandled exception");
		}
	}

	public async Task<bool> CheckForUpdates(bool userRequested, bool force)
	{
		Release? release = await Updater.GetLatestRelease();
		if (release == null || !Version.TryParse(release.TagName.TrimStart('v').Split('-')[0], out Version? releaseVersion))
			return false;

		if (releaseVersion > CurrentVersion)
		{
			if (force || PendingRelease?.TagName != release.TagName)
			{
				PendingRelease = release;
				UpdateAvailable?.Raise(this, new(release));
				ToastContentBuilder tcb = new ToastContentBuilder()
					.AddText("Update available")
					.AddText($"A new version has been released!\n{release.TagName} is now available for install. Click here to install and update!")
					.AddButton(new ToastButton("Update now", Notify.GetArguments("install-update")))
					.AddButton(new ToastButtonSnooze("Update later"))
					.AddButton(new ToastButtonDismiss());
				_ = Notify.SendNotification(tcb.Content);
				return true;
			}
		}
		else if (userRequested)
		{
			ToastContentBuilder tcb = new ToastContentBuilder()
				.AddText("No updates")
				.AddText("You are already running the latest version!");
			_ = Notify.SendNotification(tcb.Content);
		}
		return false;
	}

}
