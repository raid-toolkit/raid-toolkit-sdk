using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.UI.Xaml;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Common;
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
    public sealed partial class MainWindow : RTKWindow
    {
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
        private enum WindowLayout
        {
            Small,
            Large,
        }
        private readonly IAppUI AppUI;
        private readonly IModelLoader Loader;
        private readonly List<FrameworkElement> ContentElements = new();
        private readonly FrameworkElement DefaultContentElement;
        private WindowLayout _windowLayout = WindowLayout.Small;

        private WindowLayout Layout
        {
            get { return _windowLayout; }
            set
            {
                if (value == _windowLayout)
                    return;

                _windowLayout = value;
                switch (_windowLayout)
                {
                    case WindowLayout.Small:
                        Height = 550;
                        GrowLogoAnimation.Begin();
                        break;
                    case WindowLayout.Large:
                        ShrinkLogoAnimation.Begin();
                        break;
                }
            }
        }

        public MainWindow(
            IModelLoader loader,
            IAppUI appUI,
            IMenuManager menuManager
            )
        {
            Loader = loader;
            AppUI = appUI;
            Loader.OnStateUpdated += Loader_OnStateUpdated;
            InitializeComponent();

            AppWindow.Closing += AppWindow_Closing;

            this.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);
            IsMinimizable = false;
            IsMaximizable = false;
            IsResizable = false;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);
            IsShownInSwitchers = true;
            this.SetIsMaximizable(false);
            this.SetIsMinimizable(false);

            this.CenterOnScreen(400, 550);
#pragma warning disable CS0436 // Type conflicts with imported type
            VersionRTK.Text = ThisAssembly.AssemblyFileVersion;
#pragma warning restore CS0436 // Type conflicts with imported type
            Backdrop = new MicaSystemBackdrop();

            DefaultContentElement = LinksGrid;
            ContentElements.Add(Settings);
            ContentElements.Add(LoadProgressGrid);
            ContentElements.Add(LinksGrid);
        }

        private void Loader_OnStateUpdated(object? sender, IModelLoader.ModelLoaderEventArgs e)
        {
            AppUI.Dispatch(() =>
            {
                switch (e.LoadState)
                {
                    case IModelLoader.LoadState.Initialize:
                        {
                            ShowContent(LoadProgressGrid);
                            VersionRaid.Text = Loader.GameVersion;
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
                            if (RegistrySettings.FirstRun)
                            {
                                ShowContent(Settings);
                            }
                            else
                            {
                                ShowContent(null);
                            }
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
            if (RegistrySettings.FirstRun)
            {
                IntPtr window = this.GetWindowHandle();
                FlashWindow(window, true);
                return;
            }
            this.Hide();
            ShowContent(null);
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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        internal void OpenSettings()
        {
            ShowContent(Settings);
        }

        internal void ShowContent(FrameworkElement? control)
        {
            control ??= DefaultContentElement;
            foreach (FrameworkElement content in ContentElements)
            {
                content.Visibility = content == control ? Visibility.Visible : Visibility.Collapsed;
            }

            double targetHeight = Math.Max(control.ActualHeight, control.MinHeight);
            if (targetHeight <= 150)
            {
                Layout = WindowLayout.Small;
            }
            else
            {
                Layout = WindowLayout.Large;
                Height = Math.Max(550, targetHeight + 248);
            }
        }

        private void Settings_SettingsSaved(object sender, EventArgs e)
        {
            ShowContent(null);
        }

        private void Settings_SettingsDiscarded(object sender, EventArgs e)
        {
            ShowContent(null);
        }
    }
}
