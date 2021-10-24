using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raid.Service.Messages;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public class ModelService
    {
        private readonly IReadOnlyDictionary<string, IMessageScopeHandler> ScopeHandlers;
        private ILogger<ModelService> Logger;
        public ModelService(ILogger<ModelService> logger, IEnumerable<IMessageScopeHandler> handlers)
        {
            Logger = logger;
            ScopeHandlers = handlers.ToDictionary(handler => handler.Name);
        }

        private ValueTask ProcessMessage(WebSocketSession session, WebSocketPackage message)
        {
            Logger.LogInformation(ServiceEvent.HandleMessage.EventId(), "ProcessMessage");
            using var sessionScope = Logger.BeginScope($"[SessionId = {session.SessionID}");

            try
            {
                var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(message.Message);

                using var messageScope = Logger.BeginScope($"[Scope = {socketMessage.Scope}, Channel = {socketMessage.Channel}]");

                if (ScopeHandlers.TryGetValue(socketMessage.Scope, out IMessageScopeHandler handler))
                {
                    handler.HandleMessage(socketMessage, session);
                }
                else
                {
                    Logger.LogWarning(ServiceError.UnknownMessageScope.EventId(), $"Unknown scope '{socketMessage.Scope}'");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MessageHandlerFailure.EventId(), ex, "Failed to handle message");
            }

            return ValueTask.CompletedTask;
        }

        public static ValueTask HandleMessage(WebSocketSession session, WebSocketPackage message)
        {
            return RaidHost.Services.GetRequiredService<ModelService>().ProcessMessage(session, message);
        }
    }
}