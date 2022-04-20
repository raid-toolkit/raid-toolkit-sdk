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

namespace Raid.Toolkit.Model
{
    public class ModelLoader : IModelLoader
    {
        private static void PostfixTypes(Assembly asm)
        {
            Il2CppToolkit.Runtime.Types.Types.TypeSizes.Add(asm.GetType("Plarium.Common.Numerics.Fixed"), 8);
        }

        private static readonly Regex[] DefaultIncludeTypes = new[] {
            new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
        };

        private Version CurrentInteropVersion;
        private Regex[] IncludeTypes;

        public event EventHandler<IModelLoader.ModelLoaderEventArgs> OnStateUpdated;

        public string GameVersion { get; private set; }
        public Version InteropVersion { get; private set; }
        public string OutputDirectory { get; set; }
        public string OutputAssemblyName { get; set; } = "Raid.Interop";
        public string OutputFilename { get; set; } = "Raid.Interop.dll";
        private Assembly InteropAsm;

        public ModelLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                AssemblyName name = new(args.Name);
                if (name.Name == "Raid.Interop")
                {
                    return InteropAsm;
                }
            }
            catch (Exception) { }
            return null;
        }

        public async Task<Assembly> BuildAndLoad(IEnumerable<Regex> regices, bool force)
        {
            try
            {
                string dllPath = await Build(regices, force);
                InteropAsm = Assembly.LoadFrom(dllPath);
                PostfixTypes(InteropAsm);
                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Loaded));
                return InteropAsm;
            }
            catch (Exception)
            {
                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Error));
                throw;
            }
        }

        public async Task<string> Build(IEnumerable<Regex> regices, bool force)
        {
            try
            {
                IncludeTypes = DefaultIncludeTypes.Concat(regices).ToArray();
                var durableTypePatternList = IncludeTypes.Select(rex => rex.ToString()).Distinct().OrderBy(str => str);
                int hashCode = string.Join(";", durableTypePatternList).GetStableHashCode();

                // force a rebuild for every major.minor version bump
                Version asmVersion = Version.Parse(ThisAssembly.AssemblyVersion);
                CurrentInteropVersion = new(asmVersion.Major, asmVersion.Minor, 0, Math.Abs(hashCode % 999));

                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Initialize));

                PlariumPlayAdapter.GameInfo gameInfo = GetGameInfo();
                GameVersion = gameInfo.Version;

                string outDir = OutputDirectory ?? Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string dllPath = Path.Combine(outDir, gameInfo.Version, OutputFilename);

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

                OnStateUpdated?.Invoke(this, new(IModelLoader.LoadState.Ready));
                return dllPath;
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
            var loadedAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.FullName == OutputAssemblyName);
            ErrorHandler.Assert(loadedAsm == null, "Expected generated assembly to be unloaded!");
        }

        private void BuildAssembly(PlariumPlayAdapter.GameInfo gameInfo, string dllPath)
        {
            string metadataPath = Path.Combine(gameInfo.InstallPath, gameInfo.Version, @"Raid_Data\il2cpp_data\Metadata\global-metadata.dat");
            string gasmPath = Path.Combine(gameInfo.InstallPath, gameInfo.Version, @"GameAssembly.dll");

            Directory.CreateDirectory(Path.GetDirectoryName(dllPath));

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
                ArtifactSpecs.AssemblyName.MakeValue(OutputAssemblyName),
                ArtifactSpecs.AssemblyVersion.MakeValue(CurrentInteropVersion),
                ArtifactSpecs.OutputPath.MakeValue(dllPath)
            );

            compiler.Compile().Wait();
        }
    }
}
