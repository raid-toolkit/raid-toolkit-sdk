using System;

namespace Raid.Service
{
    public abstract class SerializableEventArgs : EventArgs
    {
        public string EventName { get; set; }
        public object[] EventArguments { get; set; }
    }

    public class AccountUpdatedEventArgs : SerializableEventArgs
    {
        public AccountUpdatedEventArgs(string accountId)
        {
            EventName = "updated";
            EventArguments = new string[] { accountId };
        }
    }
}