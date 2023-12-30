using Microsoft.UI.Dispatching;
using Microsoft.Win32.SafeHandles;
using Microsoft.Windows.AppLifecycle;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Windows.ApplicationModel.Activation;
using Windows.Win32.Security;

using WinRT;

using PInvoke = Windows.Win32.PInvoke;

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
