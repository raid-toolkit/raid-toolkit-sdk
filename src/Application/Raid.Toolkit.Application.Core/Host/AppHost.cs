using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Raid.Toolkit.Application.Core.Commands;
using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.DependencyInjection;
using Raid.Toolkit.Application.Core.Utility;
using Raid.Toolkit.Common;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Services;
using Raid.Toolkit.Extensibility.Shared;

using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

using System.Diagnostics;

namespace Raid.Toolkit.Application.Core.Host
{
    public class AppHost
    {
        private const int ActivationPollMs = 500;
        private const int ActivationTimeoutMs = 30000;

        private const string LogDir = "Logs";
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName = "Raid.Toolkit.exe";
        public static readonly string ExecutableDirectory;
        public static DeferredOptions<FileLoggerOptions> FileLoggerOptions => AppHostBuilderSettings.FileLoggerOptions;

        internal static Stream? GetEmbeddedSettings()
        {
            var assembly = typeof(AppHost).Assembly;
            return assembly.GetManifestResourceStream($"Raid.Toolkit.Application.Core.appsettings.json");
        }

        public static bool ForceRebuild { get; set; }

        public static string LogsDirectory => Path.Combine(RegistrySettings.InstallationPath, LogDir);

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
                    string logDir = Path.Combine(RegistrySettings.InstallationPath, LogDir);
                    DirectoryInfo dir = Directory.CreateDirectory(logDir);

                    IEnumerable<FileInfo> existingFiles = dir.GetFiles().Where(file => file.CreationTimeUtc < DateTime.UtcNow.AddDays(-2));
                    foreach (FileInfo file in existingFiles)
                        file.Delete();

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
            try
            {
                string oldInstallationPath = Path.Combine(RegistrySettings.InstallationPath, "Raid.Toolkit.exe");
                if (File.Exists(oldInstallationPath))
                {
                    File.Delete(oldInstallationPath);
                }
            }
            catch { }
        }

        public static async Task<int> Activate(Uri activationRequestUri, params string[] arguments)
        {
            await EnsureProcess(new() { NoLogo = true });
            RaidToolkitClientBase client = new();
            client.Connect();
            return await client.MakeApi<ActivationApi>().Activate(activationRequestUri, arguments);
        }

        private static async Task EnsureProcess(CommonOptions? options = null)
        {
            options ??= new();
            if (SingletonProcess.IsRunning)
            {
                return;
            }
            ProcessStartInfo psi = new()
            {
                FileName = ExecutablePath,
            };
            if (options.NoLogo) psi.ArgumentList.Add("--nologo");

            _ = Process.Start(psi);
            DateTime timeout = DateTime.UtcNow.AddMilliseconds(ActivationTimeoutMs);
            while (DateTime.UtcNow < timeout)
            {
                try
                {
                    RaidToolkitClientBase client = new();
                    client.Connect();
                    return;
                }
                catch
                {
                    await Task.Delay(ActivationPollMs);
                }
            }
            throw new TimeoutException();
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
