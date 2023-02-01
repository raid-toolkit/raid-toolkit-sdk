using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Markup;

using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Interfaces;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class RTKApplication : Microsoft.UI.Xaml.Application, IXamlExtensionHost
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
            await Host.Services.GetRequiredService<IAppService>().WaitForStop().ConfigureAwait(false);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs launchArgs)
        {
            //ILogger logger = Host.Services.GetRequiredService<ILogger<Bootstrap>>();
        }
    }
}
