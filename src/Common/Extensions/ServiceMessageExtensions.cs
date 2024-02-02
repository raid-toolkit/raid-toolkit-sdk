using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace System;

public static class ServiceMessageExtensions
{
    class EventIds<T> where T : System.Enum
    {
        public static IReadOnlyDictionary<T, EventId> Mapping = typeof(T).GetEnumValues().Cast<T>().ToDictionary(value => value, value => new EventId((int)(object)value, value.ToString()));
    }
    public static EventId EventId<T>(this T serviceEvent) where T : System.Enum
    {
        return EventIds<T>.Mapping[serviceEvent];
    }
}
