using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Raid.Service
{
    public static class AppConfiguration
    {
        public static readonly IConfigurationRoot Configuration;
        public static string ExecutablePath
        {
            get
            {
                return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            }
        }

        public static string ExecutableDirectory
        {
            get
            {
                return Path.GetDirectoryName(ExecutablePath);
            }
        }

        static AppConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(ExecutableDirectory)
                .AddJsonFile("appsettings.json").Build();
        }
    }
}