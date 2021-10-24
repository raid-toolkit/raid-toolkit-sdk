using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Raid.Service
{
    public enum ServiceEvent : int
    {
        ProtocolHandlerOpen = 0,
        ProtocolHandlerHandleMessage = 1,
        ProtocolHandlerAccept = 2,
        ProtocolHandlerReject = 3,
    }

    internal static class ServiceMessageExtensions
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
}