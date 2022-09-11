using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Il2CppToolkit.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

using Raid.Toolkit.App;
using Raid.Toolkit.App.Tasks.Base;
using Raid.Toolkit.DependencyInjection;
using Raid.Toolkit.Preamble.Tasks.Matchers;
using Raid.Toolkit.WinUI;

using Windows.UI;
using WinUIEx;

using SplashScreen = Raid.Toolkit.WinUI.SplashScreen;

namespace Raid.Toolkit.Preamble.Commands.Tasks
{
    internal class RunTask : ICommandTask
    {
        private readonly RunOptions Options;
        private AppInstance? Instance;
        private IHost? Host;

        public RunTask(RunOptions options)
        {
            Options = options;
        }

        public async Task<int> Invoke()
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            if (DecideRedirection())
                return 0;

            RTKApplication? app = null;

            try
            {
                AppHostBuilder hostBuilder = new();
                hostBuilder.AddWebSockets(AppHost.HandleMessage)
                    .AddExtensibility()
                    .AddLogging()
                    .AddUI()
                    .AddAppServices();
                Host = hostBuilder.Build();

                ConfigureHost(Host);

                AppHost.Start(Host);

                Application.Start((p) =>
                {
                    DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    app = new(context, Host);

                    // show splash screen to trigger app boot flow. it's dismissal
                    // will cause Start() to return flow to its calling method
                    SplashScreen splash = new();
                    splash.Show();
                    Task.Run(async () =>
                    {
                        await Host.StartAsync();
                        context.Post(_ =>
                        {
                            splash.Close();
                        }, null);
                    });
                });

                if (app == null)
                    throw new ApplicationException("Expected app to be set");

                await app.WaitForExit();
            }
            finally
            {
                if (Instance != null)
                    Instance.UnregisterKey();
            }

            // TODO: Move UserShutdown out of RTKApplication
            int exitCode = await app.UserShutdown();
            Application.Current.Exit();
            return exitCode;
        }

        private void ConfigureHost(IHost host)
        {
            IModelLoader modelLoader = host.Services.GetRequiredService<IModelLoader>();

            if (Options.DebugPackage == ".")
            {
                Options.DebugPackage = Environment.GetEnvironmentVariable("DEBUG_PACKAGE_DIR") ?? ".";
            }
            PackageManager.DebugPackage = Options.DebugPackage;
            PackageManager.NoDefaultPackages = Options.NoDefaultPackages;
            if (!string.IsNullOrEmpty(PackageManager.DebugPackage))
            {
                Options.Debug = true;
                string debugInteropDirectory = Path.Combine(PackageManager.DebugPackage, @"temp~interop");
                modelLoader.OutputDirectory = debugInteropDirectory;
            }
            if (!string.IsNullOrEmpty(Options.InteropDirectory))
            {
                modelLoader.OutputDirectory = Options.InteropDirectory;
            }
        }

        private bool DecideRedirection()
        {
            if (Options.Standalone)
                return false;

            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;

            try
            {
                AppInstance keyInstance = AppInstance.FindOrRegisterForKey("RTK");

                if (keyInstance.IsCurrent)
                {
                    Instance = keyInstance;
                    Instance.Activated += OnActivated;
                    return false;
                }

                RedirectActivationTo(args, keyInstance);
            }
            catch (Exception)
            {
                // TODO: log to eventvwr?
            }

            return true;
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

        private static IntPtr redirectEventHandle = IntPtr.Zero;

        // Do the redirection on another thread, and use a non-blocking
        // wait method to wait for the redirection to complete.
        public static void RedirectActivationTo(
            AppActivationArguments args, AppInstance keyInstance)
        {
            redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                SetEvent(redirectEventHandle);
            });
            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               new IntPtr[] { redirectEventHandle }, out uint handleIndex);
        }

        private void OnActivated(object? sender, AppActivationArguments e)
        {
            // TODO: implement service for routing activation
            // Host?.Services.GetRequiredService
            throw new NotImplementedException();
        }
    }
}
