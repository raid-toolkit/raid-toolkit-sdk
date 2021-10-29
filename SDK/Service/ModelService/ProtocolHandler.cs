using System;
using System.Web;
using Microsoft.Extensions.Logging;
using Raid.Service.UI;

namespace Raid.Service
{
    internal class ProtocolHandler : IMessageScopeHandler
    {
        public string Name => "$rtk";
        private ILogger<ProtocolHandler> Logger;
        private ChannelService ChannelService;

        public ProtocolHandler(ILogger<ProtocolHandler> logger, ChannelService channelService) =>
            (Logger, ChannelService) = (logger, channelService);

        public void HandleMessage(SocketMessage message, ISocketSession session)
        {
            Logger.LogInformation(ServiceEvent.HandleMessage.EventId(), $"Channel = {message.Channel}");

            switch (message.Channel)
            {
                case "open":
                    {
                        OpenOptions openOptions = message.Message.ToObject<OpenOptions>();
                        Logger.LogInformation(ServiceEvent.UserPermissionRequest.EventId(), $"Uri = {openOptions.Uri}");

                        Uri rtkUri = new Uri(openOptions.Uri);
                        var query = HttpUtility.ParseQueryString(rtkUri.Query);
                        var channel = query["channel"];
                        var origin = query["origin"];
                        if (PermissionsRequest.RequestPermissions(origin))
                        {
                            Logger.LogInformation(ServiceEvent.UserPermissionAccept.EventId(), "User accepted");
                            ChannelService.Accept(channel, origin);
                        }
                        else
                        {
                            Logger.LogInformation(ServiceEvent.UserPermissionReject.EventId(), "User rejected");
                            ChannelService.Reject(channel);
                        }
                        break;
                    }
            }
        }
    }
}