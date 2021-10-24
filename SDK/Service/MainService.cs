using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public class MainService : BackgroundService
    {
        private NotifyIcon notifyIcon;
        private readonly RaidInstanceFactory Factory;
        public MainService(
            ProcessWatcher processWatcher,
            UserData userData,
            StaticDataCache staticData,
            RaidInstanceFactory factory,
            IServiceProvider serviceProvider)
        {
            Factory = factory;
            processWatcher.ProcessFound += (object sender, ProcessWatcher.ProcessWatcherEventArgs e) =>
            {
                Factory.Create(e.Process, serviceProvider.CreateScope());
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Delay(-1, stoppingToken);
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
        }

        private void UpdateAccounts()
        {
            foreach (var instance in Factory.Instances.Values)
            {
                try
                {
                    instance.Update();
                }
                catch (Exception)
                {
                    // TODO: Logging
                }
            }
            TaskExtensions.RunAfter(10000, UpdateAccounts);
        }
    }
}