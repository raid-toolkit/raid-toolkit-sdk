using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.Service.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    internal class DiscoveryHandler : IMessageScopeHandler
    {
        private static string[] Types = typeof(DiscoveryHandler).Assembly.GetAttributes<PublicApiAttribute, string>((attr, _) => attr.Name).ToArray();

        public string Name => "$router/discover";

        public async void HandleMessage(SocketMessage message, ISocketSession session)
        {
            switch (message.Channel)
            {
                case "request":
                    {
                        await session.Send(new SocketMessage()
                        {
                            Scope = Name,
                            Channel = "response",
                            Message = JArray.FromObject(Types)
                        });
                        break;
                    }
                case "response":
                    {
                        break;
                    }
            }
        }
    }
}