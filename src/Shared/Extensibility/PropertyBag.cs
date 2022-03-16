using System;
using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public sealed class PropertyBag
    {
        private Dictionary<Type, object> Data = new();

        public T GetValue<T>()
        {
            if (Data.TryGetValue(typeof(T), out object value))
                return (T)value;
            return default;
        }

        public void SetValue<T>(T value)
        {
            Data[typeof(T)] = value;
        }
    }
}
