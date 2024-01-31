using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Common;

public class DisposableCollection : HashSet<IDisposable>, IDisposable
{
    private bool IsDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                Reset();
            }

            IsDisposed = true;
        }
    }

    public void Reset()
    {
        foreach (var item in this)
        {
            item.Dispose();
        }
        Clear();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
