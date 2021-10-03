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

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Raid.Extractor
{
    static class Program
    {
        static int Main(string[] args)
        {
            using (new ModelAssemblyResolver())
            {
                UseModel();
                return 0;
            }
        }

        private static void UseModel()
        {
            Process raidProc = GetRaidProcess();
            Il2CsRuntimeContext runtime = new(raidProc);
            //runtime.ObjectCreated += Runtime_ObjectCreated;
            var statics = Client.App.SingleInstance<Client.Model.AppModel>.method_get_Instance.GetMethodInfo(runtime).DeclaringClass.StaticFields
                .As<AppModelStaticFields>();
            Client.Model.AppModel appModel = statics.Instance;
            var sdm = appModel.StaticDataManager as ClientStaticDataManager;
            var instance = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver.GetInstance(runtime);
            UserWrapper userWrapper = appModel._userWrapper;
            IReadOnlyList<Artifact> artifacts;
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == ArtifactStorageMigrationState.Migrated)
            {
                var storage = instance._implementation as ExternalArtifactsStorage;
                List<Artifact> innerList = new();
                foreach ((var key, var value) in storage._state._artifacts)
                {
                    innerList.Add(value);
                }

                artifacts = innerList;
            }
            else
            {
                artifacts = userWrapper.Artifacts.ArtifactData.Artifacts;
            }
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