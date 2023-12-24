using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;

using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using Il2CppToolkit.ReverseCompiler.Target.NetCore;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raid.Toolkit.Common;

namespace Raid.Toolkit.Loader;

public class ModelLoader : IModelLoader
{
    private static void PostfixTypes(Assembly asm)
    {
        // Il2CppToolkit.Runtime.Types.TypeSystem.TypeSizes.Add(asm.GetType("Plarium.Common.Numerics.Fixed"), 8);
    }

    private static readonly Regex[] DefaultIncludeTypes = new[] {
        new Regex(@"^Client\.Model\.AppModel$", RegexOptions.Singleline | RegexOptions.Compiled),
        new Regex(@"^Plarium\.Common\.Serialization\.JsonMain", RegexOptions.Singleline | RegexOptions.Compiled),
    };

    private Version CurrentInteropVersion;
    private Regex[] IncludeTypes;
    private readonly Regex[] ExcludeTypes = new Regex[] {
        new Regex(@"^Client\.ViewModel\.Selections.*"),
        new Regex(@"^XmlNodeIdentety.*"),
        new Regex(@"^Unity\.Collections\.NativeArray`1$"),
        new Regex(@"^UnityEngine\.UIElements\.CustomStyleProperty`1$"),
        new Regex(@"^UnityEngine\.UIElements\.StyleDataRef`1$"),
        new Regex(@"^UnityEngine\.UIElements\.StyleEnum`1$"),
        new Regex(@"^Unity\.Collections\.NativeSlice`1$"),
    };

    private EventHandler<IModelLoader.ModelLoaderEventArgs> OnStateUpdatedInternal;
    public event EventHandler<IModelLoader.ModelLoaderEventArgs> OnStateUpdated
    {
        add => OnStateUpdatedInternal += value;//if (LastEvent != null)//    value(this, LastEvent);
        remove => OnStateUpdatedInternal -= value;
    }

#nullable enable
    private IModelLoader.ModelLoaderEventArgs? LastEvent;
    private void Raise(IModelLoader.ModelLoaderEventArgs eventArgs)
    {
        LastEvent = eventArgs;
        OnStateUpdatedInternal.Raise(this, eventArgs);
    }
#nullable restore

    public string GameVersion { get; private set; }
    public Version InteropVersion { get; private set; }
    public string OutputAssemblyName { get; set; } = "Raid.Interop";
    public string OutputFilename { get; set; } = "Raid.Interop.dll";
    private Assembly InteropAsm;
    private readonly ILogger<ModelLoader> Logger;
    private readonly IOptions<ModelLoaderOptions> Options;

    public ModelLoader(ILogger<ModelLoader> logger, IOptions<ModelLoaderOptions> options)
    {
        Logger = logger;
        Options = options;
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
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load interop assembly");
        }
        return null;
    }

    public async Task<Assembly> BuildAndLoad(IEnumerable<Regex> regices, string outputDirectory)
    {
        try
        {
            string dllPath = await Build(regices, outputDirectory);
            InteropAsm = Assembly.LoadFrom(dllPath);
            PostfixTypes(InteropAsm);
            Raise(new(IModelLoader.LoadState.Loaded));
            return InteropAsm;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load interop assembly");
            Raise(new(IModelLoader.LoadState.Error));
            throw;
        }
    }

    public async Task<string> Build(IEnumerable<Regex> regices, string outputDirectory)
    {
        try
        {
            IncludeTypes = DefaultIncludeTypes.Concat(regices).ToArray();
            var durableTypePatternList = ExcludeTypes.Concat(IncludeTypes).Select(rex => rex.ToString()).Distinct().OrderBy(str => str);
            int hashCode = string.Join(";", durableTypePatternList).GetStableHashCode();

            // force a rebuild for every major.minor version bump
            Version asmVersion = Version.Parse(ThisAssembly.AssemblyVersion);
            CurrentInteropVersion = new(asmVersion.Major, asmVersion.Minor, 0, Math.Abs(hashCode % 999));

            PlariumPlayAdapter.GameInfo gameInfo = GetGameInfo();
            GameVersion = gameInfo.Version;

            Raise(new(IModelLoader.LoadState.Initialize));

            string dllPath = Path.Combine(outputDirectory, gameInfo.Version, OutputFilename);

            bool shouldGenerate = Options.Value.ForceRebuild;
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
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to load interop assembly");
                shouldGenerate = true;
            }

            if (shouldGenerate)
            {
                Raise(new(IModelLoader.LoadState.Rebuild));
                await Task.Run(() =>
                {
                    GenerateAssembly(gameInfo, dllPath);
                });
            }

            Raise(new(IModelLoader.LoadState.Ready));
            return dllPath;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to load interop assembly");
            Raise(new(IModelLoader.LoadState.Error));
            throw;
        }
    }

    public static PlariumPlayAdapter.GameInfo GetGameInfo()
    {
        PlariumPlayAdapter pp = new();
        return (pp.TryGetGameVersion(101, "raid-shadow-legends", out PlariumPlayAdapter.GameInfo gameInfo)
            || pp.TryGetGameVersion(101, "raid", out gameInfo))
            ? gameInfo
            : throw new InvalidOperationException("Game is not installed");
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

        _ = Directory.CreateDirectory(Path.GetDirectoryName(dllPath));

        //
        // NB: Make sure to update CurrentInteropVersion when changing the codegen arguments!!
        //
        Il2CppToolkit.Model.Loader loader = new();
        loader.Init(gasmPath, metadataPath);
        TypeModel model = new(loader);
        Compiler compiler = new(model);
        compiler.AddTarget(new NetCoreTarget());
        compiler.AddConfiguration(
            ArtifactSpecs.TypeSelectors.MakeValue(new List<Func<TypeDescriptor, ArtifactSpecs.TypeSelectorResult>>{
                {td => IncludeTypes.Any(rex => rex.IsMatch(td.Name)) ? ArtifactSpecs.TypeSelectorResult.Include : ArtifactSpecs.TypeSelectorResult.Default },
                {td => ExcludeTypes.Any(rex => rex.IsMatch(td.Name)) ? ArtifactSpecs.TypeSelectorResult.Exclude : ArtifactSpecs.TypeSelectorResult.Default },
            }),
            ArtifactSpecs.AssemblyName.MakeValue(OutputAssemblyName),
            ArtifactSpecs.AssemblyVersion.MakeValue(CurrentInteropVersion),
            ArtifactSpecs.OutputPath.MakeValue(dllPath)
        );
        compiler.ProgressUpdated += Compiler_ProgressUpdated;

        compiler.Compile().Wait();
    }

    private void Compiler_ProgressUpdated(object sender, ProgressUpdatedEventArgs e)
    {
        Raise(new IModelLoader.ModelLoaderEventArgs(IModelLoader.LoadState.Rebuild, new IModelLoader.TaskProgress()
        {
            DisplayName = e.DisplayName,
            Completed = e.Completed,
            Total = e.Total
        }));
    }
}
public class ModelLoaderOptions
{
    public bool ForceRebuild { get; set; } = false;
}
