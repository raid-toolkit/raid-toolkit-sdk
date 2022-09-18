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
using System.Threading;

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
        private readonly IHost Host;

        public DispatcherQueueSynchronizationContext UIContext { get; }

        public static new RTKApplication Current
        {
            get => _Current ?? throw new Exception("");
        }

        public static void Post(Action action)
        {
            if (SynchronizationContext.Current == Current.UIContext)
            {
                action();
            }
            else
            {
                Current.UIContext?.Post(_ => action(), null);
            }
        }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public RTKApplication(DispatcherQueueSynchronizationContext context, IHost host)
        {
            _Current = this;
            UIContext = context;
            Host = host;
            InitializeComponent();
        }

        public async Task WaitForExit()
        {
            await Host.Services.GetRequiredService<AppService>().WaitForStop().ConfigureAwait(false);
        }

        public async Task<int> UserShutdown()
        {
            await Host.Services.GetRequiredService<AppService>().WaitForStop().ConfigureAwait(false);
            System.Windows.Forms.Application.Exit();
            IHostApplicationLifetime lifetimeService = Host.Services.GetRequiredService<IHostApplicationLifetime>();
            try
            {
                lifetimeService.StopApplication();
            }
            catch { }
            await Host.StopAsync();
            return 0;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs launchArgs)
        {
            ILogger logger = Host.Services.GetRequiredService<ILogger<Bootstrap>>();

        }
    }
}
