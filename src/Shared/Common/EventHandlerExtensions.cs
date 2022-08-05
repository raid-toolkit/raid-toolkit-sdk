using System.Linq;

namespace System
{
    public static class RTKEventHandlerExtensions
    {
        public static void Raise<T>(this EventHandler<T> eh, object sender, T e) where T : EventArgs
        {
            if (eh == null)
                return;

            foreach (EventHandler handler in eh.GetInvocationList().Cast<EventHandler>())
            {
                try
                {
                    handler(sender, e);
                }
                catch { }
            }
        }
    }
}
