using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
        static void Main(string[] args)
        {
            XamlCheckProcessRequirements();

            WinRT.ComWrappersSupport.InitializeComWrappers();
            Application.Start((p) =>
            {
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new RTKApplication(args, context);
            });
        }
    }
}
