using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static readonly Version CurrentInteropVersion;
        private static readonly string[] IncludeTypes = new[] {
            "Client.RaidApp.RaidApplication",
            "Contexts",
            "Client.ViewModel.AppViewModel",
            "Client.Model.AppModel",
            "Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage",
            "Client.Model.Gameplay.StaticData.ClientStaticDataManager",
            "SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver"
        };

        // initialize CurrentModelVersion in static ctor to ensure initialization ordering
        static ModelLoader()
        {
            int hashCode = string.Join(";", IncludeTypes).GetStableHashCode();
            CurrentInteropVersion = new(1, 3, 0, Math.Abs(hashCode % 999));
        }

        public string GameVersion { get; private set; }
        public Version InteropVersion { get; private set; }

        [Obsolete("Use GameVersion instead")]
        public string Version => GameVersion;

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
            GameVersion = gameInfo.Version;

            string executingPath = Process.GetCurrentProcess().MainModule.FileName;
            string dllPath = Path.Join(Path.GetDirectoryName(executingPath), gameInfo.Version, "Raid.Interop.dll");

            bool shouldGenerate = force;
            try
            {
                if (File.Exists(dllPath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(dllPath);
                    Version onDiskVersion = new(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
                    if (onDiskVersion != CurrentInteropVersion)
                    {
                        shouldGenerate = true;
                    }
                }
                else
                {
                    shouldGenerate = true;
                }
            }
            catch (Exception)
            {
                shouldGenerate = true;
            }

            if (shouldGenerate)
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

            //
            // NB: Make sure to update CurrentInteropVersion when changing the codegen arguments!!
            //
            Loader loader = new();
            loader.Init(gasmPath, metadataPath);
            TypeModel model = new(loader);
            Compiler compiler = new(model);
            compiler.AddTarget(new NetCoreTarget());
            compiler.AddConfiguration(
                ArtifactSpecs.TypeSelectors.MakeValue(new List<Func<TypeDescriptor, bool>>{
                    {td => IncludeTypes.Contains(td.Name)},
                }),
                ArtifactSpecs.AssemblyName.MakeValue("Raid.Interop"),
                ArtifactSpecs.AssemblyVersion.MakeValue(CurrentInteropVersion),
                ArtifactSpecs.OutputPath.MakeValue(dllPath)
            );

            compiler.Compile().Wait();
        }
    }
}