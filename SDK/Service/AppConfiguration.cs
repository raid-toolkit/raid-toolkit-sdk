using System;
using Microsoft.Extensions.Configuration;

namespace Raid.Service
{
    public static class AppConfiguration
    {
        public static readonly IConfigurationRoot Configuration;

        static AppConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
        }
    }
}