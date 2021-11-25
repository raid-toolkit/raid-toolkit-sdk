using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using Il2CppToolkit.ReverseCompiler.Target.NetCore;

namespace Raid.Model
{
    internal class ModelLoader
    {
        public string Version { get; private set; }

        private class CollectibleAssemblyLoadContext : AssemblyLoadContext
        {
            public CollectibleAssemblyLoadContext() : base(isCollectible: true)
            { }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                return null;
            }
        }


        internal Assembly Load(bool force = false)
        {
            PlariumPlayAdapter.GameInfo gameInfo = GetGameInfo();
            Version = gameInfo.Version;

            string executingPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string dllPath = Path.Join(Path.GetDirectoryName(executingPath), gameInfo.Version, "Raid.Interop.dll");

            if (!File.Exists(dllPath) || force)
            {
                GenerateAssembly(gameInfo, dllPath);
            }

            return Assembly.LoadFrom(dllPath);
        }

        public static PlariumPlayAdapter.GameInfo GetGameInfo()
        {
            PlariumPlayAdapter pp = new();
            return !pp.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo)
                ? throw new InvalidOperationException("Game is not installed")
                : gameInfo;
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
            Compiler compiler = new(model);
            compiler.AddTarget(new NetCoreTarget());
            compiler.AddConfiguration(
                ArtifactSpecs.TypeSelectors.MakeValue(new List<Func<TypeDescriptor, bool>>{
                    {td => td.Name == "Client.ViewModel.AppViewModel"},
                    {td => td.Name == "Client.Model.AppModel"},
                    {td => td.Name == "Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage"},
                    {td => td.Name == "Client.Model.Gameplay.StaticData.ClientStaticDataManager"},
                    {td => td.Name == "SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver"}
                }),
                ArtifactSpecs.AssemblyName.MakeValue("Raid.Interop"),
                ArtifactSpecs.OutputPath.MakeValue(dllPath)
            );

            compiler.Compile().Wait();
        }
    }
}