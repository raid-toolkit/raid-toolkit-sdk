using Microsoft.UI.Dispatching;
using System.Threading;

namespace Raid.Toolkit;

internal static class Program
{
	private static string CleanArgument(string arg) => arg switch
	{
		"-Embedding" => "--Embedding",
		"----AppNotificationActivated:" => "----AppNotificationActivated",
		_ => arg,
	};
	[STAThread]
	private static int Main(string[] args)
	{
		WinRT.ComWrappersSupport.InitializeComWrappers();
		AppRouter router = new();
		if (router.Instance.IsCurrent)
		{
			Microsoft.UI.Xaml.Application.Start((p) =>
			{
				var context = new DispatcherQueueSynchronizationContext(
					DispatcherQueue.GetForCurrentThread());
				SynchronizationContext.SetSynchronizationContext(context);
				RTKApplication app = new(router.Options);
				router.Activated += (_, args) => app.OnActivated(args);
				app.OnActivated(router.Options);
			});
		}
		return 0;
	}
}
