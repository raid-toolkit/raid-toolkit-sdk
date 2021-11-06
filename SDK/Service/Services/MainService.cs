using System;
using System.Diagnostics;
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
        private readonly UpdateService UpdateService;
        private GitHub.Schema.Release LatestRelease;

        public MainService(
            ProcessWatcherService processWatcher,
            ILogger<MainService> logger,
            RaidInstanceFactory factory,
            UpdateService updateService,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime,
            MemoryLogger memoryLogger)
        {
            Factory = factory;
            Logger = logger;
            Lifetime = appLifetime;
            UpdateService = updateService;
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            processWatcher.ProcessFound += (object sender, ProcessWatcherService.ProcessWatcherEventArgs e) =>
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

        private void OnUpdateAvailable(object sender, UpdateService.UpdateAvailbleEventArgs e)
        {
            if (LatestRelease.TagName == e.Release.TagName)
                return; // already notified for this update

            LatestRelease = e.Release;
            notifyIcon.ShowBalloonTip(10000, "Update available", $"A new version has been released!\n{e.Release.TagName} is now available for install. Click here to install and update!", ToolTipIcon.Info);
            if (notifyIcon.ContextMenuStrip.Items.Count == 1)
            {
                notifyIcon.ContextMenuStrip.Items.Add("Install update", null, OnUpdateClicked);
            }
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

        public void Restart()
        {
            Process.Start(AppConfiguration.ExecutablePath, new string[] { "--wait", "30000" });
            Exit();
        }

        public void Exit()
        {
            notifyIcon.Dispose();
            TaskExtensions.Shutdown();
            Application.Exit();
            Lifetime.StopApplication();
        }

        private void CreateTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppConfiguration.ExecutablePath);
            notifyIcon.Text = "Raid Toolkit";
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, OnClose);
            notifyIcon.BalloonTipClicked += OnUpdateClicked;
            notifyIcon.Visible = true;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            if (LatestRelease == null)
                return;

            try
            {
                await UpdateService.InstallRelease(LatestRelease);
                Restart();
            }
            catch { }
        }

        private void OnClose(object sender, EventArgs e)
        {
            Exit();
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