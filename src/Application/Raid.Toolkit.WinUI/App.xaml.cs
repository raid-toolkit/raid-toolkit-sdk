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
using System.Threading.Tasks;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class RTKApplication : Microsoft.UI.Xaml.Application
    {
        private static RTKApplication? _Current = null;
        private readonly IHost host;
        private readonly CommandTaskManager ApplicationStartupTask;
        private readonly ApplicationStartupCondition StartCondition;
        public int ExitCode { get; private set; }
        private readonly string[] Arguments;

        public DispatcherQueueSynchronizationContext UIContext { get; }

        public static new RTKApplication Current
        {
            get => _Current ?? throw new Exception("");
        }
        

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public RTKApplication(string[] arguments, DispatcherQueueSynchronizationContext context)
        {
            _Current = this;
            UIContext = context;

            Arguments = arguments;
            if (Arguments.Contains("--quiet") || Arguments.Contains("-q"))
            {
                Arguments = Arguments.Where(arg => arg is not ("--quiet" or "-q")).ToArray();
                AppHost.EnableLogging = false;
            }

            host = AppHost.CreateHost();

            ApplicationStartupTask = host.Services.GetRequiredService<CommandTaskManager>();
            StartCondition = ApplicationStartupTask.Parse(Arguments);

            InitializeComponent();
        }

        public async Task WaitForExit()
        {
            await host.Services.GetRequiredService<AppService>().WaitForStop().ConfigureAwait(false);
        }

        public async Task<int> UserShutdown()
        {
            await host.Services.GetRequiredService<AppService>().WaitForStop().ConfigureAwait(false);
            System.Windows.Forms.Application.Exit();
            IHostApplicationLifetime lifetimeService = host.Services.GetRequiredService<IHostApplicationLifetime>();
            if (StartCondition.HasFlag(ApplicationStartupCondition.Services))
            {
                try
                {
                    lifetimeService.StopApplication();
                }
                catch { }
                await host.StopAsync();
            }
            return ExitCode;
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
            ILogger logger = host.Services.GetRequiredService<ILogger<Bootstrap>>();

            if (StartCondition.HasFlag(ApplicationStartupCondition.Usage))
            {
                Exit(255);
                return;
            }

            if (StartCondition.HasFlag(ApplicationStartupCondition.Services))
            {
                SplashScreen splash = new();
                splash.Show();
                Task.Run(async () =>
                {
                    await host.StartAsync();
                    //UIContext.Post(_ =>
                    //{
                    //    splash.Close();
                    //}, null);
                });
            }
            try
            {
                ApplicationStartupTask.Execute();
            }
            catch (Exception e)
            {
                string errorMessage = "A fatal error occurred";
                logger.LogError(e, errorMessage);
                if ((Arguments.Contains("--debug") || Arguments.Contains("-d")) && !Arguments.Contains("--no-ui") && !Arguments.Contains("-n"))
                {
                    errorMessage += $":\n\n{e.Message}\n{e.StackTrace}";
                }
                _ = MessageBox.Show(new Form(), errorMessage, "Raid Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Exit(1);
                return;
            }
            finally
            {
            }
        }
    }
}
