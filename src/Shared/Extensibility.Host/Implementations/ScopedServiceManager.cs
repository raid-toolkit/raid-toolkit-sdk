using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Host.Services;

namespace Raid.Toolkit.Extensibility.Services
{
    public class ScopedServiceManager : IScopedServiceManager
    {
        private readonly List<IMessageScopeHandler> ScopeHandlers = new();
        private readonly ILogger<ScopedServiceManager> Logger;

        public ScopedServiceManager(ILogger<ScopedServiceManager> logger)
        {
            Logger = logger;
        }

        public ValueTask ProcessMessage(ISocketSession session, string message)
        {
            // TODO: Add permissions check

            // string? origin = session.HttpHeader.Items.Get("origin");
            // if (!string.IsNullOrEmpty(origin))
            // {
            //     if (!Host!.Services.GetRequiredService<UI.MainWindow>().RequestPermissions(origin))
            //     {
            //         session.CloseAsync(CloseReason.ViolatePolicy, "User denied access");
            //     }
            // }
            try
            {
                var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(message);

                var messageScope = Logger.BeginScope($"[Scope = {socketMessage.Scope}, Channel = {socketMessage.Channel}]");
                Logger.LogDebug(ServiceEvent.HandleMessage.EventId(), "HandleMessage");

                IMessageScopeHandler handler = ScopeHandlers.FirstOrDefault(handler => handler.SupportsScope(socketMessage.Scope));
                if (handler != null)
                {
                    Logger.LogDebug(ServiceEvent.HandleMessage.EventId(), $"Dispatch message to {handler.GetType().FullName}");
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

        public void AddMessageScopeHandler(IMessageScopeHandler handler)
        {
            ScopeHandlers.Add(handler);
        }

        public void RemoveMessageScopeHandler(IMessageScopeHandler handler)
        {
            ScopeHandlers.Remove(handler);
        }
    }
}
