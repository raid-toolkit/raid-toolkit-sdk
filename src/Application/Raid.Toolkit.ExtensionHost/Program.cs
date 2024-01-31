using Microsoft.UI.Dispatching;
using System.Threading;

namespace Raid.Toolkit.ExtensionHost;

class Program
{
	[STAThread]
	static int Main()
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
				App app = new(router.Options);
				router.Activated += (_, args) => app.OnActivated(args);
				app.OnActivated(router.Options);
			});
		}
		return 0;
	}
}
