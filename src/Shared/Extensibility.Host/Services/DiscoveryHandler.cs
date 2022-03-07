using Newtonsoft.Json.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility.Services;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    internal class DiscoveryHandler : IMessageScopeHandler
    {
        private string Name => "$router/discover";

        private readonly IScopedServiceManager ServiceManager;
        public DiscoveryHandler(IScopedServiceManager serviceManager)
        {
            ServiceManager = serviceManager;
        }

        private string[] Types => ServiceManager.SupportedApis;

        public bool SupportsScope(string scopeName)
        {
            return scopeName == Name;
        }

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
