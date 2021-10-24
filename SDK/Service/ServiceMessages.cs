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
        private static IReadOnlyDictionary<ServiceEvent, EventId> EventIds = typeof(ServiceEvent).GetEnumValues().Cast<ServiceEvent>().ToDictionary(value => value, value => new EventId((int)value, value.ToString()));
        public static EventId EventId(this ServiceEvent serviceEvent)
        {
            return EventIds[serviceEvent];
        }
    }
}