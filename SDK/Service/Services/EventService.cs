using System;

namespace Raid.Service
{
    public class EventService : IdleBackgroundService
    {
        public event EventHandler<AccountUpdatedEventArgs> OnAccountUpdated;

        public void EmitAccountUpdated(string accountId)
        {
            OnAccountUpdated?.Invoke(this, new(accountId));
        }
    }
}
