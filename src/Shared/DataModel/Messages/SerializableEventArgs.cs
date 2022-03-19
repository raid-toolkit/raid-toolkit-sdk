using System;

namespace Raid.Toolkit.DataModel
{
    public abstract class SerializableEventArgs : EventArgs
    {
        public string EventName { get; set; }
        public object[] EventArguments { get; set; }
    }

    public class BasicSerializableEventArgs : SerializableEventArgs
    {
        public BasicSerializableEventArgs(string eventName, params string[] arguments)
        {
            EventName = eventName;
            EventArguments = arguments;
        }
    }

    public class AccountUpdatedEventArgs : SerializableEventArgs
    {
        public AccountUpdatedEventArgs(string accountId)
        {
            EventName = "updated";
            EventArguments = new string[] { accountId };
        }
    }

    public class ViewUpdatedEventArgs : SerializableEventArgs
    {
        public ViewUpdatedEventArgs(string accountId, string viewKey)
        {
            EventName = "view-changed";
            EventArguments = new string[] { accountId, viewKey };
        }
    }

}
