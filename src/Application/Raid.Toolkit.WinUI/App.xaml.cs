using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Raid.Toolkit.App.Tasks;
using Raid.Toolkit.App;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.UI.Dispatching;
using Windows.UI;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class RTKApplication : Microsoft.UI.Xaml.Application
    {
        public int ExitCode { get; private set; }
        private readonly string[] Arguments;

        public static DispatcherQueueSynchronizationContext? UIContext;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public RTKApplication(string[] arguments, DispatcherQueueSynchronizationContext context)
        {
            Arguments = arguments;
            UIContext = context;
            InitializeComponent();
        }

        private void Exit(int exitCode)
        {
            ExitCode = exitCode;
            Exit();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs launchArgs)
        {
            string[] args = Arguments;
            if (args.Contains("--quiet") || args.Contains("-q"))
            {
                args = args.Where(arg => arg is not ("--quiet" or "-q")).ToArray();
                AppHost.EnableLogging = false;
            }

            using IHost host = AppHost.CreateHost();
            ILogger logger = host.Services.GetRequiredService<ILogger<Bootstrap>>();
            ApplicationStartupTask applicationStartupTask = host.Services.GetRequiredService<ApplicationStartupTask>();
            ApplicationStartupCondition startCondition = applicationStartupTask.Parse(args);

            if (startCondition.HasFlag(ApplicationStartupCondition.Usage))
            {
                Exit(255);
                return;
            }

            IntPtr asyncEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            if (startCondition.HasFlag(ApplicationStartupCondition.Services))
            {
                host.StartAsync();
                //host.StartAsync().ContinueWith((_) =>
                //{
                //    SetEvent(asyncEventHandle);
                //});
                //uint CWMO_DEFAULT = 0;
                //uint INFINITE = 0xFFFFFFFF;
                //_ = CoWaitForMultipleObjects(
                //   CWMO_DEFAULT, INFINITE, 1,
                //   new IntPtr[] { asyncEventHandle }, out uint handleIndex);
            }
            try
            {
                int exitCode = applicationStartupTask.Execute();
                Exit(exitCode);
            }
            catch (Exception e)
            {
                string errorMessage = "A fatal error occurred";
                logger.LogError(e, errorMessage);
                if ((args.Contains("--debug") || args.Contains("-d")) && !args.Contains("--no-ui") && !args.Contains("-n"))
                {
                    errorMessage += $":\n\n{e.Message}\n{e.StackTrace}";
                }
                _ = MessageBox.Show(new Form(), errorMessage, "Raid Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit(1);
                return;
            }
            finally
            {
                if (startCondition.HasFlag(ApplicationStartupCondition.Services))
                {
                    try
                    {
                        host.Services.GetService<IHostApplicationLifetime>()?.StopApplication();
                    }
                    catch { }
                    host.StopAsync().Wait();
                }
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateEvent(
            IntPtr lpEventAttributes, bool bManualReset,
            bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("ole32.dll")]
        private static extern uint CoWaitForMultipleObjects(
            uint dwFlags, uint dwMilliseconds, ulong nHandles,
            IntPtr[] pHandles, out uint dwIndex);
    }
}
