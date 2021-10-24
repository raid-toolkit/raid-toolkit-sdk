using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Raid.Service.Messages;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public class ModelService
    {
        private readonly IReadOnlyDictionary<string, IMessageScopeHandler> ScopeHandlers;
        public ModelService(IEnumerable<IMessageScopeHandler> handlers)
        {
            ScopeHandlers = handlers.ToDictionary(handler => handler.Name);
        }

        private ValueTask ProcessMessage(WebSocketSession session, WebSocketPackage message)
        {
            var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(message.Message);

            if (ScopeHandlers.TryGetValue(socketMessage.Scope, out IMessageScopeHandler handler))
            {
                handler.HandleMessage(socketMessage, session);
            }

            // TODO: Error handling/logging
            return ValueTask.CompletedTask;
        }

        public static ValueTask HandleMessage(WebSocketSession session, WebSocketPackage message)
        {
            return RaidHost.Services.GetRequiredService<ModelService>().ProcessMessage(session, message);
        }
    }
}