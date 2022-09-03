using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Extensibility.Host.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace Raid.Toolkit.UI.Windows
{
    /// <summary>
    /// Interaction logic for ErrorsWindow.xaml
    /// </summary>
    public partial class ErrorsWindow : Window
    {
        private readonly AppService AppService;
        private readonly ErrorService ErrorService;
        private readonly FileLoggerOptions? LoggerSettings;
        private readonly string? LogDirectory;

        public ErrorsWindow(ErrorService errorService, AppService appService, IOptions<FileLoggerOptions> loggerSettings)
        {
            InitializeComponent();
            AppService = appService;
            ErrorService = errorService;
            if (AppHost.EnableLogging)
            {
                LoggerSettings = loggerSettings.Value;
                LogDirectory = System.IO.Path.Combine(LoggerSettings.RootPath, LoggerSettings.BasePath);
            }
            RefreshData();
        }

        private void RefreshData()
        {
            ErrorList = ErrorService.CurrentErrors.Values.ToArray();

            if (string.IsNullOrEmpty(LogDirectory))
                return;

            string[] allFiles = System.IO.Directory.GetFiles(LogDirectory);
            if (allFiles.Length == 0)
                return;

            string currentLog = allFiles.Select(file => new { Created = System.IO.File.GetCreationTimeUtc(file), Path = file }).OrderByDescending(file => file.Created).First().Path;
            {
                using var fs = new System.IO.FileStream(currentLog, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                using var sr = new System.IO.StreamReader(fs);
                LogText = sr.ReadToEnd();
            }
        }

        public static readonly DependencyProperty ErrorListProperty =
            DependencyProperty.Register("ErrorList", typeof(ErrorEventArgs[]), typeof(ErrorsWindow));

        public ErrorEventArgs[] ErrorList
        {
            get => (ErrorEventArgs[])GetValue(ErrorListProperty);
            set => SetValue(ErrorListProperty, value);
        }

        public static readonly DependencyProperty LogTextProperty =
            DependencyProperty.Register("LogText", typeof(string), typeof(ErrorsWindow));

        public string LogText
        {
            get => (string)GetValue(LogTextProperty);
            set => SetValue(LogTextProperty, value);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void RestartAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            AppService.Restart(postUpdate: false, asAdmin: true, owner: new WindowWrapper(this));
        }

        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            // https://stackoverflow.com/questions/10296018/get-system-windows-forms-iwin32window-from-wpf-window

            public WindowWrapper(IntPtr handle)
            {
                Handle = handle;
            }

            public WindowWrapper(Window window)
            {
                Handle = new WindowInteropHelper(window).Handle;
            }

            public IntPtr Handle { get; }
        }
    }
}
