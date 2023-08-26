using Client.Model.Gameplay.Artifacts;
using Il2CppToolkit.Runtime;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extension.Account;

public abstract class AccountDataExtensionBase : IAccountExtension, IAccountExtensionService, IDisposable
{
    protected readonly IAccount Account;
    protected readonly IExtensionStorage Storage;
    protected readonly ILogger Logger;

    protected bool IsDisposed;

    public AccountDataExtensionBase(IAccount account, IExtensionStorage storage, ILogger logger)
    {
        Account = account;
        Storage = storage;
        Logger = logger;
    }

    public virtual void OnConnected(Il2CsRuntimeContext runtime) { }

    public virtual void OnDisconnected() { }

    protected abstract Task Update(ModelScope scope);

    public bool HasWork { get; protected set; } = true;

    public virtual async Task OnTick()
    {
        if (!Account.IsOnline)
            return;

        ModelScope scope = new(Account.Runtime);

        await Update(scope);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
