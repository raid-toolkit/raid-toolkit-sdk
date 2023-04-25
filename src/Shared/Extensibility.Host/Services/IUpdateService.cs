using System;
using System.Threading;
using System.Threading.Tasks;
using GitHub.Schema;
using Microsoft.Extensions.Hosting;

namespace Raid.Toolkit.Extensibility.Host.Services
{
    public class UpdateAvailableEventArgs : EventArgs
    {
        public Release Release { get; private set; }
        public UpdateAvailableEventArgs(Release release)
        {
            Release = release;
        }
    }
    public interface IUpdateService
    {
        bool IsEnabled { get; }
        event EventHandler<UpdateAvailableEventArgs>? UpdateAvailable;
        Task InstallUpdate();
        Task InstallRelease(Release release);
        Task<bool> CheckForUpdates(bool userRequested, bool force);
    }
    public class UpdateServiceStub : IUpdateService, IHostedService
    {
        public bool IsEnabled => false;

        public event EventHandler<UpdateAvailableEventArgs>? UpdateAvailable;

        public Task InstallUpdate()
        {
            return Task.CompletedTask;
        }

        public Task InstallRelease(Release release)
        {
            return Task.CompletedTask;
        }

        public Task<bool> CheckForUpdates(bool userRequested, bool force)
        {
            return Task.FromResult(false);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
