using System;
using System.Web;
using Raid.Service.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    internal class ProtocolHandler : IMessageScopeHandler
    {
        public string Name => "$rtk";

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
                        ChannelService.Instance.Accept(channel, origin);
                        break;
                    }
            }
        }
    }
}