using System;
using System.IO;
using System.Linq;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using System.Reflection;
using System.Runtime.Loader;
using Il2CppToolkit.Common.Errors;

namespace Raid.Model
{
    internal class ModelLoader
    {
        class CollectibleAssemblyLoadContext : AssemblyLoadContext
        {
            public CollectibleAssemblyLoadContext() : base(isCollectible: true)
            { }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return null;
            }
        }

        internal Assembly Load()
        {
            PlariumPlayAdapter pp = new();
            if (!pp.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo))
            {
                throw new InvalidOperationException("Game is not installed");
            }

            string executingPath = Assembly.GetExecutingAssembly().Location;
            string dllPath = Path.Join(Path.GetDirectoryName(executingPath), gameInfo.Version, "Raid.Interop.dll");

            if (!File.Exists(dllPath))
            {
                GenerateAssembly(gameInfo, dllPath);
            }

            return Assembly.LoadFrom(dllPath);
        }

        private static void GenerateAssembly(PlariumPlayAdapter.GameInfo gameInfo, string dllPath)
        {
            // separated into separate method to ensure we can GC the generated ASM
            BuildAssembly(gameInfo, dllPath);
            GC.Collect();
            var loadedAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == "Raid.Interop");
            ErrorHandler.Assert(loadedAsm == null, "Expected generated assembly to be unloaded!");
        }

        private static void BuildAssembly(PlariumPlayAdapter.GameInfo gameInfo, string dllPath)
        {
            string metadataPath = Path.Join(gameInfo.InstallPath, gameInfo.Version, @"Raid_Data\il2cpp_data\Metadata\global-metadata.dat");
            string gasmPath = Path.Join(gameInfo.InstallPath, gameInfo.Version, @"GameAssembly.dll");

            Loader loader = new();
            loader.Init(gasmPath, metadataPath);
            TypeModel model = new(loader);
            AssemblyGenerator asmGen = new(model);
            asmGen.TypeSelectors.Add(td => td.Name == "Client.Model.AppModel");
            asmGen.TypeSelectors.Add(td => td.Name == "Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage");
            asmGen.TypeSelectors.Add(td => td.Name == "Client.Model.Gameplay.StaticData.ClientStaticDataManager");
            asmGen.TypeSelectors.Add(td => td.Name == "SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver");
            asmGen.AssemblyName = "Raid.Interop";
            asmGen.OutputPath = dllPath;
            asmGen.GenerateAssembly().Wait();
        }
    }
}