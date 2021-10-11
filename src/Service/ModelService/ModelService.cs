using Microsoft.Extensions.Hosting;
using SuperSocket.ProtoBase;
using SuperSocket.WebSocket.Server;

namespace Raid.Service
{
    public class ModelService
    {
        private IHost m_host;
        public ModelService()
        {
            m_host = WebSocketHostBuilder.Create().Build();
        }
    }
}