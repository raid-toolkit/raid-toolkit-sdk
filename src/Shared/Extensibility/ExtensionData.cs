using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Raid.Toolkit.Extensibility
{
    public abstract class ExtensionDataStorage : Dictionary<string, object>
    {
        private readonly IExtensionStorage Storage;
        private readonly string Id;

        protected ExtensionDataStorage(IExtensionStorage storage, string id)
        {
            Storage = storage;
            Id = id;
            if (Storage.TryRead<Dictionary<string, object>>(Id, out var dict))
            {
                foreach (var kvp in dict)
                    Add(kvp.Key, kvp.Value);
            }
        }
        protected void SetValue<T>(T value, [CallerMemberName] string name = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            this[name] = value;
            Storage.Write(Id, this);
        }

        protected T GetValue<T>(T defaultValue = default, [CallerMemberName] string name = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return !TryGetValue(name, out object value) ? defaultValue : (T)value;
        }
    }
}
