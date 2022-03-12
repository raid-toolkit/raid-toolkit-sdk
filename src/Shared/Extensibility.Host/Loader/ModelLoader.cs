using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using Il2CppToolkit.ReverseCompiler.Target.NetCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host
{
    public class ModelLoader : IModelLoader
    {
        private static void PostfixTypes()
        {
            Il2CppToolkit.Runtime.Types.Types.TypeSizes.Add(typeof(Plarium.Common.Numerics.Fixed), 8);
        }

        private static readonly Regex[] DefaultIncludeTypes = new[] {
            //new Regex(@"^Client\.ViewModel\.Contextes\.", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Client\.View\.Views\.", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Entitas\.", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^ECS\.(Components|ViewModel)\.", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Client\.RaidApp\.RaidViewMaster$", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Client\.RaidApp\.RaidApplication$", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Contexts$", RegexOptions.Singleline | RegexOptions.Compiled),
            //new Regex(@"^Client\.ViewModel\.AppViewModel$", RegexOptions.Singleline | RegexOptions.Compiled),
            new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
        };


        private Version CurrentInteropVersion;
        private Regex[] IncludeTypes;

        public event EventHandler<IModelLoader.ModelLoaderEventArgs> OnStateUpdated;

        public string GameVersion { get; private set; }
        public Version InteropVersion { get; private set; }

        public Task<Assembly> Load(IEnumerable<Regex> regices, bool force)
        {
            IncludeTypes = DefaultIncludeTypes.Concat(regices).ToArray();
            int hashCode = string.Join(";", IncludeTypes.Select(rex => rex.ToString())).GetStableHashCode();
            // force a rebuild for every major.minor version bump
            Version asmVersion = Version.Parse(ThisAssembly.AssemblyVersion);
            CurrentInteropVersion = new(asmVersion.Major, asmVersion.Minor, 0, Math.Abs(hashCode % 999));
            return Load(force);
        }

        private async Task<Assembly> Load(bool force)
        {
            try
            {
                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Initialize));

                PlariumPlayAdapter.GameInfo gameInfo = GetGameInfo();
                GameVersion = gameInfo.Version;

                string executingPath = Process.GetCurrentProcess().MainModule.FileName;
                string dllPath = Path.Combine(Path.GetDirectoryName(executingPath), gameInfo.Version, "Raid.Interop.dll");

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
                    OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Rebuild));
                    await Task.Run(() =>
                    {
                        GenerateAssembly(gameInfo, dllPath);
                    });
                }

                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Load));
                //ModelLoaderContext loaderContext = new(dllPath);
                //Assembly asm = loaderContext.LoadFromAssemblyPath(dllPath);
                Assembly asm = Assembly.LoadFrom(dllPath);
                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Ready));
                PostfixTypes();
                return asm;
            }
            catch (Exception)
            {
                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Error));
                throw;
            }
        }

        public static PlariumPlayAdapter.GameInfo GetGameInfo()
        {
            PlariumPlayAdapter pp = new();
            return !pp.TryGetGameVersion(101, "raid", out PlariumPlayAdapter.GameInfo gameInfo)
                ? throw new InvalidOperationException("Game is not installed")
                : gameInfo;
        }

        private void GenerateAssembly(PlariumPlayAdapter.GameInfo gameInfo, string dllPath)
        {
            // separated into separate method to ensure we can GC the generated ASM
            BuildAssembly(gameInfo, dllPath);
            GC.Collect();
            var loadedAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == "Raid.Interop");
            ErrorHandler.Assert(loadedAsm == null, "Expected generated assembly to be unloaded!");
        }

        private void BuildAssembly(PlariumPlayAdapter.GameInfo gameInfo, string dllPath)
        {
            string metadataPath = Path.Combine(gameInfo.InstallPath, gameInfo.Version, @"Raid_Data\il2cpp_data\Metadata\global-metadata.dat");
            string gasmPath = Path.Combine(gameInfo.InstallPath, gameInfo.Version, @"GameAssembly.dll");

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
                    {td => IncludeTypes.Any(rex => rex.IsMatch(td.Name)) }
                }),
                ArtifactSpecs.AssemblyName.MakeValue("Raid.Interop"),
                ArtifactSpecs.AssemblyVersion.MakeValue(CurrentInteropVersion),
                ArtifactSpecs.OutputPath.MakeValue(dllPath)
            );

            compiler.Compile().Wait();
        }
    }
}
