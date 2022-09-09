using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Raid.Toolkit.WinUI;

namespace Raid.Toolkit
{
    /// <summary>
    /// Program class
    /// </summary>
    public static class Program
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        [DebuggerNonUserCode]
        [STAThread]
        static int Main(string[] args)
        {
            XamlCheckProcessRequirements();

            RTKApplication? app = null;
            WinRT.ComWrappersSupport.InitializeComWrappers();
            int exitCode = 255;
            Application.Start(async (p) =>
            {
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                app = new(args, context);
                exitCode = await app.WaitForExit();
                Application.Current.Exit();
            });

            return exitCode;
        }
    }
}
