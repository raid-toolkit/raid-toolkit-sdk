using System.Collections.Generic;

namespace Raid.Toolkit.Extensibility
{
    public sealed class PropertyBag
    {
        private Dictionary<string, object> Data = new();

        public T GetValue<T>()
        {
            if (Data.TryGetValue(typeof(T).FullName, out object value))
                return (T)value;
            return default;
        }

        public T GetValue<T>(string key)
        {
            if (Data.TryGetValue($"{typeof(T).FullName}, {key}", out object value))
                return (T)value;
            return default;
        }

        public void SetValue<T>(T value)
        {
            Data[typeof(T).FullName] = value;
        }

        public void SetValue<T>(string key, T value)
        {
            Data[$"{typeof(T).FullName}, {key}"] = value;
        }
    }
}
