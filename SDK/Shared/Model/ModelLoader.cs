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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Model
{
    internal class ModelLoader
    {
        private static readonly Version CurrentInteropVersion;
        private static readonly Regex[] IncludeTypes = new[] {
            new Regex(@"^Client\.ViewModel\.Contextes\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.View\.Views\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Entitas\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^ECS\.(Components|ViewModel)\.", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.RaidApp\.RaidViewMaster$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.RaidApp\.RaidApplication$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Contexts$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.ViewModel\.AppViewModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.Artifacts\.ExternalArtifactsStorage$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.Gameplay\.StaticData\.ClientStaticDataManager$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^SharedModel\.Meta\.Artifacts\.ArtifactStorage\.ArtifactStorageResolver$", RegexOptions.Singleline | RegexOptions.Compiled)
        };

        // initialize CurrentModelVersion in static ctor to ensure initialization ordering
        static ModelLoader()
        {
            int hashCode = string.Join(";", IncludeTypes.Select(rex => rex.ToString())).GetStableHashCode();
            CurrentInteropVersion = new(1, 3, 0, Math.Abs(hashCode % 999));
        }

        public string GameVersion { get; private set; }
        public Version InteropVersion { get; private set; }

        public event EventHandler<EventArgs> OnRebuildStarted;
        public event EventHandler<EventArgs> OnRebuildCompleted;

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

        internal Task<Assembly> Load(bool force = false)
        {
            return Load(_ => { }, force);
        }

        internal async Task<Assembly> Load(Action<ModelLoadState> stateChangeCallback, bool force = false)
        {
            try
            {
                stateChangeCallback(ModelLoadState.Initialize);

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
                    stateChangeCallback(ModelLoadState.Rebuild);
                    await Task.Run(() =>
                    {
                        GenerateAssembly(gameInfo, dllPath);
                    });
                }

                stateChangeCallback(ModelLoadState.Load);
                return Assembly.LoadFrom(dllPath);
            }
            catch (Exception)
            {
                stateChangeCallback(ModelLoadState.Error);
                throw;
            }
            finally
            {
                stateChangeCallback(ModelLoadState.Ready);
            }
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
                    {td => IncludeTypes.Any(rex => rex.IsMatch(td.Name))}
                }),
                ArtifactSpecs.AssemblyName.MakeValue("Raid.Interop"),
                ArtifactSpecs.AssemblyVersion.MakeValue(CurrentInteropVersion),
                ArtifactSpecs.OutputPath.MakeValue(dllPath)
            );

            compiler.Compile().Wait();
        }
    }
}
