using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raid.Service
{
    public class MainService : BackgroundService
    {
        private NotifyIcon notifyIcon;
        private readonly RaidInstanceFactory Factory;
        private readonly ILogger<MainService> Logger;
        private readonly IHostApplicationLifetime Lifetime;
        public MainService(
            ProcessWatcher processWatcher,
            ILogger<MainService> logger,
            RaidInstanceFactory factory,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime)
        {
            Factory = factory;
            Logger = logger;
            Lifetime = appLifetime;
            processWatcher.ProcessFound += (object sender, ProcessWatcher.ProcessWatcherEventArgs e) =>
            {
                try
                {
                    Factory.Create(e.Process, serviceProvider.CreateScope());
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccoutNotReady.EventId(), ex, "Account is not ready");
                    e.Retry = true;
                }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch { }
        }

        public void Run(RunOptions options)
        {
            if (!options.Standalone)
                RegisterAction.RegisterProtocol(true);

            if (!options.NoUI)
                CreateTrayIcon();

            Console.CancelKeyPress += (sender, e) => Application.Exit();
            TaskExtensions.RunAfter(1, UpdateAccounts);
            Application.Run();
        }

        private void CreateTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppConfiguration.ExecutablePath);
            notifyIcon.Text = "Raid Toolkit";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, OnClose);
            notifyIcon.Visible = true;
        }

        private void OnClose(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            TaskExtensions.Shutdown();
            Application.Exit();
            Lifetime.StopApplication();
        }

        private void UpdateAccounts()
        {
            foreach (var instance in Factory.Instances.Values)
            {
                try
                {
                    instance.Update();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ServiceError.AccountUpdateFailed.EventId(), ex, $"Failed to update account {instance.Id}");
                }
            }
            TaskExtensions.RunAfter(10000, UpdateAccounts);
        }
    }
}