using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Il2CppToolkit.Runtime;
using Raid.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Raid.Extractor
{
    static class Program
    {
        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = false
                },
            },
        };

        static int Main(string[] args)
        {
            Process raidProc = GetRaidProcess();
            using (new ModelAssemblyResolver())
            {
                var dump = Extract(raidProc);
                var json = JsonConvert.SerializeObject(dump, Formatting.Indented, SerializerSettings);
                File.WriteAllText("artifacts.json", json);
                return 0;
            }
        }

        private static RaidExtractor.Core.AccountDump Extract(Process process)
        {
            Extractor extractor = new(process);

            return extractor.Extract();
        }


        private static Process GetRaidProcess()
        {
            Process process = Process.GetProcessesByName("Raid").FirstOrDefault();
            if (process == null)
            {
                throw new Exception("Raid needs to be running before running RaidExtractor");
            }

            return process;
        }

        [Size(16)]
        public struct AppModelStaticFields
        {
            [Offset(8)]
            public Client.Model.AppModel Instance;
        }
    }
}