using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly List<string> SupportedApiList = new();

        public string[] SupportedApis => SupportedApiList.ToArray();

        public ScopedServiceManager(ILogger<ScopedServiceManager> logger)
        {
            Logger = logger;
            ScopeHandlers.Add(new DiscoveryHandler(this));
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
            foreach (var attr in handler.GetType().GetInterfaces().Select(type => type.GetCustomAttribute<PublicApiAttribute>(true)))
            {
                if (attr != null)
                    SupportedApiList.Add(attr.Name);
            }
        }

        public void RemoveMessageScopeHandler(IMessageScopeHandler handler)
        {
            ScopeHandlers.Remove(handler);
            foreach (var attr in handler.GetType().GetInterfaces().Select(type => type.GetCustomAttribute<PublicApiAttribute>(true)))
            {
                if (attr != null)
                    SupportedApiList.Remove(attr.Name);
            }
        }
    }
}
