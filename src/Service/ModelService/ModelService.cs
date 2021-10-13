using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Raid.Service.Messages;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public class ModelService
    {
        private IReadOnlyDictionary<string, IMessageScopeHandler> m_scopeHandlers;
        private IHost m_host;

        public ModelService()
        {
            m_scopeHandlers = typeof(ModelService).Assembly.ConstructTypesAssignableTo<IMessageScopeHandler>().ToDictionary(handler => handler.Name);
            m_host = WebSocketHostBuilder.Create()
                .UseWebSocketMessageHandler(ProcessMessage)
                .ConfigureAppConfiguration((hostCtx, configApp) =>
                {
                    configApp.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "serverOptions:name", "raid-toolkit-service" },
                        { "serverOptions:listeners:0:ip", "Any" },
                        { "serverOptions:listeners:0:port", "9090" }
                    });
                })
                .Build();
        }

        public async void Start()
        {
            await m_host.StartAsync();
        }

        public async void Stop()
        {
            await m_host.StopAsync();
        }

        private ValueTask ProcessMessage(WebSocketSession session, WebSocketPackage message)
        {
            var socketMessage = JsonConvert.DeserializeObject<SocketMessage>(message.Message);
            if (m_scopeHandlers.TryGetValue(socketMessage.Scope, out IMessageScopeHandler handler))
            {
                handler.HandleMessage(socketMessage, session);
            }
            // TODO: Error handling/logging
            return ValueTask.CompletedTask;
        }
    }
}