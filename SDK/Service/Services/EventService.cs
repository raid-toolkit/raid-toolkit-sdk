using System;

namespace Raid.Service
{
    public class EventService : IdleBackgroundService
    {
        public event EventHandler<AccountUpdatedEventArgs> OnAccountUpdated;
        public event EventHandler<ViewUpdatedEventArgs> OnViewUpdated;

        public void EmitAccountUpdated(string accountId)
        {
            OnAccountUpdated?.Invoke(this, new(accountId));
        }

        public void EmitViewUpdated(string accountId, string viewKey)
        {
            OnViewUpdated?.Invoke(this, new(accountId, viewKey));
        }
    }
}
