using System.Diagnostics;

using Microsoft.UI.Xaml;

using Raid.Toolkit.Extensibility;
using Raid.Toolkit.UI.WinUI.Base;

using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : RTKWindow
    {
        private readonly IModelLoader Loader;
        private readonly IMenuManager MenuManager;

        public SplashScreen(
            IModelLoader loader,
            IMenuManager menuManager
            )
        {
            Loader = loader;
            MenuManager = menuManager;
            Loader.OnStateUpdated += Loader_OnStateUpdated;
            InitializeComponent();

            this.SetTitleBarBackgroundColors(Microsoft.UI.Colors.Purple);
            AppWindow.Closing += AppWindow_Closing;

            IsShownInSwitchers = true;
            IsMinimizable = false;
            IsMaximizable = false;
            IsResizable = false;
            this.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);
            this.CenterOnScreen(400, 450);
#pragma warning disable CS0436 // Type conflicts with imported type
            VersionRTK.Text = ThisAssembly.AssemblyFileVersion;
#pragma warning restore CS0436 // Type conflicts with imported type
            Backdrop = new MicaSystemBackdrop();
        }

        private void Loader_OnStateUpdated(object? sender, IModelLoader.ModelLoaderEventArgs e)
        {
            RTKApplication.Post(() =>
            {
                switch (e.LoadState)
                {
                    case IModelLoader.LoadState.Initialize:
                        {
                            Height = 600;
                            VersionRaid.Text = Loader.GameVersion;
                            LoadProgressGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                            LoadMessage.Text = "Initializing...";
                        }
                        break;
                    case IModelLoader.LoadState.Rebuild:
                        {
                            LoadMessage.Text = "Raid Toolkit detected a new game version and needs to rebuild the game model before it can run.";
                            if (e.Progress != null)
                            {
                                LoadStatus.Text = e.Progress.DisplayName ?? "";
                                LoadProgress.Maximum = e.Progress.Total;
                                LoadProgress.Value = e.Progress.Completed;
                            }
                        }
                        break;
                    case IModelLoader.LoadState.Ready:
                        {
                            LoadMessage.Text = "Raid.Interop loaded!";
                        }
                        break;
                    case IModelLoader.LoadState.Loaded:
                        {
                            Height = 510;
                            LoadProgressGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                            LinksGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                        }
                        break;
                    case IModelLoader.LoadState.Error:
                        {
                            // TODO: click to close experience
                            LoadMessage.Text = "An error occurred";
                        }
                        break;
                    default:
                        break;
                }
            });
        }

        private void AppWindow_Closing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
        {
            args.Cancel = true;
            this.Hide();
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "https://github.com/raid-toolkit/raid-toolkit-sdk",
                UseShellExecute = true,
                Verb = "Open"
            };
            Process.Start(psi);
        }

        private void Discord_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new()
            {
                FileName = "https://discord.gg/9vqjMs3wAC",
                UseShellExecute = true,
                Verb = "Open"
            };
            Process.Start(psi);
        }
    }
}
