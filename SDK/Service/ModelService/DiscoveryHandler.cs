using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raid.Service.Messages;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    internal class DiscoveryHandler : IMessageScopeHandler
    {
        public string Name => "$router/discover";

        private static string[] Types;
        static DiscoveryHandler()
        {
            Types = typeof(DiscoveryHandler).Assembly.GetTypesWithAttribute<PublicApiAttribute>().Select(tuple => tuple.Item2.Name).ToArray();
        }

        public async void HandleMessage(SocketMessage message, WebSocketSession session)
        {
            switch (message.Channel)
            {
                case "request":
                    {
                        await session.SendAsync(JsonConvert.SerializeObject(new SocketMessage()
                        {
                            Scope = Name,
                            Channel = "response",
                            Message = JArray.FromObject(Types)
                        }));
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