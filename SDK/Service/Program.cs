using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Service
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            string appSettingsPath = Path.Join(AppConfiguration.ExecutableDirectory, "appsettings.json");
            if (!File.Exists(appSettingsPath))
            {
                File.WriteAllText(appSettingsPath, GetBuiltInSettings().ToString());
            }

            Type t = typeof(Newtonsoft.Json.JsonSerializer);
            return Parser.Default.ParseArguments<RegisterOptions, OpenOptions, RunOptions>(args)
                .MapResult<RegisterOptions, OpenOptions, RunOptions, Task<int>>(RegisterAction.Execute, OpenAction.Execute, RunAction.Execute, HandleErrors);
        }

        private static Task<int> HandleErrors(IEnumerable<Error> _)
        {
            return Task.FromResult(1);
        }

        private static JToken GetBuiltInSettings()
        {
            var serializer = new JsonSerializer();
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json"))
            using (var sr = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sr))
            {
                return JObject.ReadFrom(textReader);
            }
        }
    }
}