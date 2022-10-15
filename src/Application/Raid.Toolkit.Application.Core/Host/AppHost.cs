using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.DependencyInjection;
using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Utility;
using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Extensibility.Shared;


using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Toolkit.Application.Core.Host
{
    public class AppHost
    {
        private const string LogDir = "Logs";
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName = "Raid.Toolkit.exe";
        public static readonly string ExecutableDirectory;
        public static DeferredOptions<FileLoggerOptions> FileLoggerOptions => AppHostBuilderSettings.FileLoggerOptions;

        public static void EnsureFileAssociations(string? exePath = null)
        {
            if (string.IsNullOrEmpty(exePath))
                exePath = ExecutablePath;
            FileAssociations.EnsureAssociationsSet(new FileAssociation()
            {
                Extension = ".rtkx",
                FileTypeDescription = "Raid Toolkit Extension",
                ExecutableFilePath = exePath,
                ExecutableArguments = "install",
                ProgId = "RTK.Extension"
            });
        }

        internal static Stream? GetEmbeddedSettings()
        {
            var assembly = typeof(AppHost).Assembly;
            return assembly.GetManifestResourceStream($"Raid.Toolkit.Application.Core.appsettings.json");
        }

        public static bool ForceRebuild { get; set; }

        private static bool _EnableLogging = false;
        public static bool EnableLogging
        {
            get => _EnableLogging;
            set
            {
                if (value == _EnableLogging)
                    return;

                _EnableLogging = value;
                if (!_EnableLogging)
                {
                    FileLoggerOptions.Set(new());
                    return;
                }
                if (Directory.Exists(RegistrySettings.InstallationPath))
                {
                    Directory.CreateDirectory(Path.Combine(RegistrySettings.InstallationPath, LogDir));
                    PhysicalFileProvider fileProvider = new(RegistrySettings.InstallationPath);
                    FileLoggerOptions options = new()
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
                        Files = new[]
                        {
                        new LogFileOptions
                        {
                            Path = "<date:yyyyMMdd>-<counter>.log",
                        },
                    },
                    };
                    FileLoggerOptions.Set(options);
                }
            }
        }

        static AppHost()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!;
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath)!;
        }

        private static IHost? Host;

        public static void Start(IHost host)
        {
            Host = host;
        }

        /**
         * Handles messages from the public socket API
        **/
        public static ValueTask HandleMessage(WebSocketSession session, WebSocketPackage message)
        {
            return Host == null
                ? ValueTask.CompletedTask
                : Host.Services.GetRequiredService<IScopedServiceManager>()
                .ProcessMessage(new SuperSocketAdapter(session), message.Message);
        }
    }
}
