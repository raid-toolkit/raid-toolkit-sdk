using System;

namespace Raid.Service
{
    public abstract class BaseSerializableEventArgs : EventArgs
    {
        public abstract object ObjectValue { get; }
    }

    public class SerializableEventArgs<T> : BaseSerializableEventArgs
    {
        public override object ObjectValue => Value;
        public T Value { get; set; }
    }
}