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
            switch (message.Channel)
            {
                case "open":
                    {
                        OpenOptions openOptions = message.Message.ToObject<OpenOptions>();
                        Uri rtkUri = new Uri(openOptions.Uri);
                        var query = HttpUtility.ParseQueryString(rtkUri.Query);
                        var channel = query["channel"];
                        var origin = query["origin"];
                        if (PermissionsRequest.RequestPermissions(origin))
                        {
                            ChannelService.Accept(channel, origin);
                        }
                        else
                        {
                            ChannelService.Reject(channel);
                        }
                        break;
                    }
            }
        }
    }
}