using System;
using System.Web;
using Microsoft.Extensions.Logging;
using Raid.Service.Messages;
using Raid.Service.UI;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    internal class ProtocolHandler : IMessageScopeHandler
    {
        public string Name => "$rtk";
        private ILogger<ProtocolHandler> Logger;
        private ChannelService ChannelService;

        public ProtocolHandler(ILogger<ProtocolHandler> logger, ChannelService channelService) =>
            (Logger, ChannelService) = (logger, channelService);

        public void HandleMessage(SocketMessage message, WebSocketSession session)
        {
            using var scope = Logger.BeginScope($"SessionId = {session.SessionID}");

            Logger.LogInformation(ServiceEvent.ProtocolHandlerHandleMessage.EventId(), $"Channel = {message.Channel}");

            try
            {
                switch (message.Channel)
                {
                    case "open":
                        {
                            OpenOptions openOptions = message.Message.ToObject<OpenOptions>();
                            Logger.LogInformation(ServiceEvent.ProtocolHandlerOpen.EventId(), $"Uri = {openOptions.Uri}");

                            Uri rtkUri = new Uri(openOptions.Uri);
                            var query = HttpUtility.ParseQueryString(rtkUri.Query);
                            var channel = query["channel"];
                            var origin = query["origin"];
                            if (PermissionsRequest.RequestPermissions(origin))
                            {
                                Logger.LogInformation(ServiceEvent.ProtocolHandlerAccept.EventId(), "User accepted");
                                ChannelService.Accept(channel, origin);
                            }
                            else
                            {
                                Logger.LogInformation(ServiceEvent.ProtocolHandlerReject.EventId(), "User rejected");
                                ChannelService.Reject(channel);
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ServiceError.MessageProcessingFailure.EventId(), ex, "Failed to handle message");
            }
        }
    }
}