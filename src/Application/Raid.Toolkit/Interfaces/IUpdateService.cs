using GitHub.Schema;

namespace Raid.Toolkit;

public class UpdateAvailableEventArgs : EventArgs
{
	public Release Release { get; private set; }
	public UpdateAvailableEventArgs(Release release)
	{
		Release = release;
	}
}

public interface IUpdateService
{
	bool IsEnabled { get; }
	event EventHandler<UpdateAvailableEventArgs>? UpdateAvailable;
	Task InstallUpdate();
	Task InstallRelease(Release release);
	Task<bool> CheckForUpdates(bool userRequested, bool force);
}
