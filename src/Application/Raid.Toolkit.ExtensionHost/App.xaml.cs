using CustomExtensions.WinUI;

using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Utils;
using Raid.Toolkit.Loader;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.ExtensionHost;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private const string LogDir = "Logs";

    private Window? m_window;

    public App()
    {
        this.InitializeComponent();
        ApplicationExtensionHost.Initialize(this);
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Hide();
    }

    internal void OnActivated(BaseOptions options)
    {
        switch (options)
        {
            case RunExtensionOptions runOptions:
                Task.Run(() => RunExtension(runOptions));
                break;
        }
    }

    private async Task RunExtension(RunExtensionOptions options)
    {
        HostBuilder hostBuilder = new();

        // logging
        hostBuilder
            .ConfigureLogging((_) => GetLoggerOptions(options))
            .ConfigureServices((context, services) => services
                .AddLogging(builder => builder.AddFile())
                .Configure<ModelLoaderOptions>(config => config.ForceRebuild = options.ForceRebuild)
                .AddExtensibilityServices<PackageManager>());
        IHost host = hostBuilder.Start();
        IExtensionHostController controller = host.Services.GetRequiredService<IExtensionHostController>();

        if (!controller.TryGetExtension(options.GetExtensionId(), out IExtensionManagement? extension))
            throw new InvalidOperationException("Extension not found");

        if (extension.State == ExtensionState.None)
            await extension.Load();

        if (extension.State != ExtensionState.Loaded)
            throw new InvalidOperationException("Extension was not loaded properly");

        extension.Activate();
        extension.ShowUI();
    }

    private static FileLoggerOptions GetLoggerOptions(BaseOptions options)
    {
        if (!Directory.Exists(RegistrySettings.InstallationPath))
        {
            return new();
        }
        string logDir = Path.Combine(RegistrySettings.InstallationPath, LogDir);
        DirectoryInfo dir = Directory.CreateDirectory(logDir);

        IEnumerable<FileInfo> existingFiles = dir.GetFiles().Where(file => file.CreationTimeUtc < DateTime.UtcNow.AddDays(-2));
        foreach (FileInfo file in existingFiles)
            file.Delete();

        string logFileNameFormat = $"Extension.{options.GetExtensionId()}.<date:yyyyMMdd>-<counter>.log";

        PhysicalFileProvider fileProvider = new(RegistrySettings.InstallationPath);
        FileLoggerOptions loggerOptions = new()
        {
            FileAppender = new PhysicalFileAppender(fileProvider),
            BasePath = LogDir,
            FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush,
            FileEncodingName = "utf-8",
            DateFormat = "yyyyMMdd",
            CounterFormat = "000",
            MaxFileSize = 10485760,
            IncludeScopes = true,

            TextBuilder = new SingleLineLogEntryTextBuilder(),
            Files = new[] { new LogFileOptions { Path = logFileNameFormat } },
        };
        return loggerOptions;
    }
}
