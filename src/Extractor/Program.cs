using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using Il2CppToolkit.Runtime;
using Microsoft.Win32;
using Raid.Model;

using Client.Model.Gameplay.Artifacts;
using Client.Model.Gameplay.Heroes;
using Client.Model.Gameplay.Heroes.Data;
using Client.Model.Gameplay.StaticData;
using Client.Model.Guard;
using SharedModel.Meta.Artifacts;
using SharedModel.Meta.Artifacts.ArtifactStorage;
using SharedModel.Meta.Heroes;
using SharedModel.Meta.Heroes.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

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