using System;
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
    public class ModelService : IDisposable
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
                    configApp.AddJsonFile("appsettings.json");
                })

                .Build();
        }

        public void Dispose()
        {
            m_host.Dispose();
        }

        public async void Start()
        {
            await m_host.StartAsync();
        }

        public async Task Stop()
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