using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using Il2CppToolkit.Common.Errors;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.Preamble;
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

        [STAThread]
        static async Task<int> Main(string[] args)
        {
            CommonOptions.Parse(args);
            AppHost.EnableLogging = CommonOptions.Value?.DisableLogging ?? true;

            Entrypoint entry = new();
            CommandTaskManager commandManager = entry.CreateInstance<CommandTaskManager>();
            ICommandTask? task = commandManager.Parse(args);
            if (task == null)
                return 255;

            return await task.Invoke();

            //XamlCheckProcessRequirements();

            //RTKApplication? app = null;
            //int exitCode = 255;

            //WinRT.ComWrappersSupport.InitializeComWrappers();

            //Application.Start((p) =>
            //{
            //    DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
            //    SynchronizationContext.SetSynchronizationContext(context);
            //    app = new(args, context);
            //});

            //if (app == null)
            //    throw new ApplicationException("Expected app to be set");

            //await app.WaitForExit();
            //exitCode = await app.UserShutdown();
            //Application.Current.Exit();

            //return exitCode;
        }
    }
}
