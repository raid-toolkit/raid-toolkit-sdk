using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Options
{
#nullable enable
    class DeferredOptions<TOptions> : IOptionsMonitor<TOptions>
    {
        protected static object _Lock = new();
        private readonly Dictionary<string, TOptions> Values = new();
        private Action<TOptions, string>? Listeners;
        private static string DefaultName => Options.DefaultName;

        public TOptions CurrentValue
        {
            get => Values[DefaultName];
            set
            {
                Values[DefaultName] = value;
            }
        }

        public DeferredOptions()
        { }

        public DeferredOptions(TOptions options)
        {
            Values.Add(DefaultName, options);
        }

        public void Set(TOptions value, string? name = null)
        {
            lock (_Lock)
            {
                Values[name ?? DefaultName] = value;
                Listeners?.Invoke(value, name ?? DefaultName);
            }
        }

        public TOptions Get(string? name = null)
        {
            lock (_Lock)
            {
                return Values.TryGetValue(name ?? DefaultName, out TOptions? value)
                    ? value
                    : throw new ArgumentOutOfRangeException(nameof(name));
            }
        }

        public IDisposable OnChange(Action<TOptions, string> listener)
        {
            lock (_Lock)
            {
                Listeners += listener;
            }
            return new ChangeRegistrationToken(this, listener);
        }

        private class ChangeRegistrationToken : IDisposable
        {
            private DeferredOptions<TOptions>? Owner;
            private Action<TOptions, string>? Listener;

            public ChangeRegistrationToken(DeferredOptions<TOptions> owner, Action<TOptions, string> listener)
            {
                Owner = owner;
                Listener = listener;
            }

            public void Dispose()
            {
                lock (_Lock)
                {
                    if (Listener != null && Owner != null)
                    {
                        Owner.Listeners -= Listener;
                        Owner = null;
                        Listener = null;
                    }
                }
            }
        }
    }
#nullable restore
}
