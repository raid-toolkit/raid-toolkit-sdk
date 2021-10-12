using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public class ModelService
    {
        private IHost m_host;
        public ModelService()
        {
            var host = WebSocketHostBuilder.Create()
                .UseWebSocketMessageHandler(ProcessMessage)
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "raid-toolkit-service" },
                        { "serverOptions:listeners:0:ip", "127.0.0.1" },
                        { "serverOptions:listeners:0:port", "9090" }
                    });
                })
                .Build();
        }

        private static async ValueTask ProcessMessage(WebSocketSession session, WebSocketPackage message)
        {
            var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(message.Message);
            await session.SendAsync(message.Message);
        }
    }
}