using Client.Model.Gameplay.Artifacts;

using Il2CppToolkit.Runtime;

using Microsoft.Extensions.Logging;

using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public class ArtifactExtension :
    AccountDataExtensionBase,
    IAccountPublicApi<IGetAccountDataApi<ArtifactsDataObject>>,
    IGetAccountDataApi<ArtifactsDataObject>,
    IAccountExportable
{
    private const string Key = "artifacts.json";
    private readonly ArtifactsProviderState State;

    IGetAccountDataApi<ArtifactsDataObject> IAccountPublicApi<IGetAccountDataApi<ArtifactsDataObject>>.GetApi() => this;
    bool IGetAccountDataApi<ArtifactsDataObject>.TryGetData(out ArtifactsDataObject data) => Storage.TryRead(Key, out data);

    public ArtifactExtension(IAccount account, IExtensionStorage storage, ILogger<ArtifactExtension> logger)
    : base(account, storage, logger)
    {
        State = new();
    }

    protected override Task Update(ModelScope scope)
    {
        var artifactData = scope.AppModel._userWrapper.Artifacts.ArtifactData;

        IReadOnlyList<Artifact> artifacts;

        bool hasUpdates = true;

        if (State.ShouldForceUpdate() || !Storage.TryRead(Key, out ArtifactsDataObject previous))
        {
            Logger.LogInformation("Performing full artifact update");
            artifacts = GetArtifacts(scope);

        }
        else if (previous != null && State.ShouldIncrementalUpdate(artifactData))
        {
            Logger.LogInformation("Performing incremental artifact update");
            artifacts = previous.Values.ToList();
            hasUpdates = false;
        }
        else
        {
            Logger.LogDebug("Skipping artifact update");
            return Task.CompletedTask;
        }

        ArtifactsDataObject result = new();

        var updatedArtifacts = artifactData.UpdatedArtifacts;
        var deletedArtifacts = artifactData.DeletedArtifactIds;

        foreach (var artifactEntry in artifacts)
        {
            if (artifactEntry == null) continue;
            if (deletedArtifacts.Contains(artifactEntry.Id))
            {
                hasUpdates = true;
                continue;
            }

            if (updatedArtifacts.TryGetValue(artifactEntry.Id, out var artifact))
            {
                result.Add(artifactEntry.Id, artifact.ToModel());
                hasUpdates = true;
            }
            else
            {
                result.Add(artifactEntry.Id, artifactEntry);
            }
        }

        if (hasUpdates)
        {
            Storage.Write(Key, result);
            State.MarkRefresh(artifactData);
        }

        return Task.CompletedTask;
    }

    public void Export(IAccountReaderWriter account)
    {
        if (Storage.TryRead(Key, out ArtifactsDataObject data))
            account.Write(Key, data);
    }

    public void Import(IAccountReaderWriter account)
    {
        if (account.TryRead(Key, out ArtifactsDataObject? data))
            Storage.Write(Key, data);
    }

    private static IReadOnlyList<Artifact> GetArtifacts(ModelScope scope)
    {
        Client.Model.Guard.UserWrapper userWrapper = scope.AppModel._userWrapper;
        if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
        {
            ExternalArtifactsStorage? storage = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver._implementation.GetValue(scope.Context) as ExternalArtifactsStorage;
            return storage?._state._artifacts.Values.Select(ModelExtensions.ToModel).ToArray() ?? Array.Empty<Artifact>();
        }
        return userWrapper.Artifacts.ArtifactData.Artifacts.Select(ModelExtensions.ToModel).ToList();
    }
}
