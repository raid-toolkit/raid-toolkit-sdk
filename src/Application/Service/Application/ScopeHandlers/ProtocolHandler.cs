using System;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.DataModel;

namespace Raid.Service
{
    internal class ProtocolHandler : IMessageScopeHandler
    {
        private string Name => "$rtk";
        private readonly ILogger<ProtocolHandler> Logger;
        private readonly ChannelService ChannelService;
        private readonly IServiceProvider ServiceProvider;

        public ProtocolHandler(ILogger<ProtocolHandler> logger, ChannelService channelService, IServiceProvider serviceProvider) =>
            (Logger, ChannelService, ServiceProvider) = (logger, channelService, serviceProvider);

        public bool SupportsScope(string scopeName)
        {
            return scopeName == Name;
        }

        public void HandleMessage(SocketMessage message, ISocketSession session)
        {
            Logger.LogInformation(ServiceEvent.HandleMessage.EventId(), $"Channel = {message.Channel}");

            switch (message.Channel)
            {
                case "open":
                    {
                        OpenOptions openOptions = message.Message.ToObject<OpenOptions>();
                        Logger.LogInformation(ServiceEvent.UserPermissionRequest.EventId(), $"Uri = {openOptions.Uri}");

                        Uri rtkUri = new(openOptions.Uri);
                        var query = HttpUtility.ParseQueryString(rtkUri.Query);
                        var channel = query["channel"];
                        var origin = query["origin"];
                        if (ServiceProvider.GetRequiredService<UI.MainWindow>().RequestPermissions(origin))
                        {
                            Logger.LogInformation(ServiceEvent.UserPermissionAccept.EventId(), "User accepted");
                            ChannelService.Accept(channel, origin);
                        }
                        else
                        {
                            Logger.LogInformation(ServiceEvent.UserPermissionReject.EventId(), "User rejected");
                            ChannelService.Reject(channel);
                        }
                        break;
                    }
            }
        }

    }
}
