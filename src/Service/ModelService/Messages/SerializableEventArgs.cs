using System;

namespace Raid.Service
{
    public class SerializableEventArgs : EventArgs
    {
        public string EventName { get; set; }
        public object[] EventArguments { get; set; }
    }

}