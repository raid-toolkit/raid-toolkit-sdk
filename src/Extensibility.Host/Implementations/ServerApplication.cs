using Microsoft.Extensions.Hosting;

using Raid.Toolkit.Common.API;
using Raid.Toolkit.Common.API.Messages;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.IPC;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Host;

public class ServerApplication : IServerApplication, IHostedService
{
    private static readonly ApiMessageSerializer Serializer = new();
    public IPCServer<SocketMessage>? Server { get; private set; }
    private readonly List<IApiServer<SocketMessage>> Servers = new();

    public void RegisterApiServer(IApiServer<SocketMessage> server)
    {
        Servers.Add(server);
    }

    private void Server_MessageReceived(object? sender, SocketMessage e)
    {
        if (sender is not IApiSession<SocketMessage> socket)
            return;
        foreach (var server in Servers.Where(server => server.SupportsScope(e.Scope)))
        {
            server.HandleMessage(e, socket);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Server = new(Constants.IPCPipeName, Serializer);
        Server.MessageReceived += Server_MessageReceived;
        Server.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Server?.Dispose();
        Server = null;
        return Task.CompletedTask;
    }
}
