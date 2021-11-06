using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Raid.Service
{
    public static class AppConfiguration
    {
        private static readonly Lazy<IConfigurationRoot> _configuration = new(
            () => new ConfigurationBuilder()
                .SetBasePath(ExecutableDirectory)
                .AddJsonFile("appsettings.json").Build()
            );
        public static IConfigurationRoot Configuration => _configuration.Value;
        public static readonly string ExecutablePath;
        public static readonly string ExecutableName;
        public static readonly string ExecutableDirectory;
        public static readonly Version AppVersion = new Version(ThisAssembly.AssemblyFileVersion);

        static AppConfiguration()
        {
            ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            ExecutableName = Path.GetFileName(ExecutablePath);
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath);
        }
    }
}