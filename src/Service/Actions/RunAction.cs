using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raid.Model;

namespace Raid.Service
{
    static class RunAction
    {
        private static ModelService modelService;
        private static NotifyIcon notifyIcon;

        public static int Execute(RunOptions options)
        {
            if (!options.Standalone)
            {
                options.NoUI = false;
            }
            using (var mutex = new Mutex(false, "RaidToolkit Singleton"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                if (isAnotherInstanceOpen && !options.Standalone)
                {
                    return 1;
                }

                try
                {
                    Run(options).Wait();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            Application.ExitThread();
            return 0;
        }

        private static async Task Run(RunOptions options)
        {
            if (!options.NoUI)
            {
                CreateTrayIcon();
            }
            Console.CancelKeyPress += delegate
            {
                Application.Exit();
            };
            var task = Task.Run(() =>
            {
                using (new ModelAssemblyResolver())
                {
                    ProcessWatcher processWatcher = new("Raid");
                    processWatcher.ProcessFound += ProcessFound;
                    TaskExtensions.RunAfter(2000, UpdateAccounts);

                    modelService = new();
                    modelService.Start();
                }
            });
            Application.Run();
            await modelService.Stop();
            modelService.Dispose();
        }

        private static void CreateTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Text = "Raid Toolkit";
            // notifyIcon.MouseClick += ClickTrayIcon;
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Close", null, OnClose);
            notifyIcon.Visible = true;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            TaskExtensions.Shutdown();
            Application.Exit();
        }

        private static void UpdateAccounts()
        {
            foreach (var instance in RaidInstance.Instances)
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

        private static void ProcessFound(object sender, ProcessWatcher.ProcessWatcherEventArgs e)
        {
            RaidInstance instance = new(e.Process);
        }
    }
}