using Microsoft.Extensions.Hosting;

namespace Raid.Toolkit.Application.Core
{
    public interface IProgramHost
    {
        public Task Start(IHost host, Action startupFunction);
    }
}
