using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Raid.DataModel;
namespace Raid.Service
{
    internal class DiscoveryHandler : IMessageScopeHandler
    {
        private string Name => "$router/discover";
        private static string[] Types;

        static DiscoveryHandler()
        {
            var candidateTypes = typeof(DiscoveryHandler).Assembly
                .GetTypesAssignableTo<IMessageScopeHandler>()
                .Where(type => !type.IsAbstract);
            var typesToProcess = candidateTypes.Concat(candidateTypes.SelectMany(type => type.GetInterfaces()));
            List<string> apiNames = new();
            foreach (Type type in typesToProcess)
            {
                PublicApiAttribute attr = type.GetCustomAttribute<PublicApiAttribute>(true);
                if (attr == null)
                    continue;

                apiNames.Add(attr.Name);
            }
            Types = apiNames.ToArray();
        }

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
