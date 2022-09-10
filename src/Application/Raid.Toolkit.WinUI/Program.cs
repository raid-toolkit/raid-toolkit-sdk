using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Il2CppToolkit.Common.Errors;
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
        static async Task<int> Main(string[] args)
        {
            XamlCheckProcessRequirements();

            RTKApplication? app = null;
            int exitCode = 255;

            WinRT.ComWrappersSupport.InitializeComWrappers();

            Application.Start((p) =>
            {
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                app = new(args, context);
            });

            if (app == null)
                throw new ApplicationException("Expected app to be set");

            await app.WaitForExit();
            exitCode = await app.UserShutdown();
            Application.Current.Exit();

            return exitCode;
        }
    }
}
