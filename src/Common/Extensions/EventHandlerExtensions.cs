using System;
using System.Linq;

namespace Raid.Toolkit.Common;

public static class RTKEventHandlerExtensions
{
    public static void Raise<T>(this EventHandler<T> eh, object sender, T e) where T : EventArgs
    {
        if (eh == null)
            return;

        foreach (EventHandler<T> handler in eh.GetInvocationList().Cast<EventHandler<T>>())
        {
            try
            {
                handler(sender, e);
            }
            catch { }
        }
    }
}
