using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Client.ViewModel.Notifications;

using Raid.Toolkit.Application.Core.Commands.Base;
using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host.Services;
using Raid.Toolkit.Extensibility.Interfaces;
using Raid.Toolkit.Extensibility.Notifications;
using Raid.Toolkit.UI.WinUI.Forms;

namespace Raid.Toolkit.UI.WinUI
{
    public partial class AppTray
    {
        public class AppTrayMenu : ContextMenuStrip
        {
            private const int kDefaultBalloonTipTimeout = 10000;

            private void aboutMenuItem_Click(object? sender, EventArgs e)
            {
                AppUI.ShowMain();
            }

            private void settingsMenuItem_Click(object? sender, EventArgs e)
            {
                AppUI.ShowSettings();
            }

            private void viewErrorsToolStripMenuItem_Click(object? sender, EventArgs e)
            {
                AppUI.ShowErrors();
            }

            private void appTrayMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
            {
                extensionsToolStripMenuItem.DropDownItems.Clear();
                extensionsToolStripMenuItem.DropDownItems.AddRange(MenuManager.GetEntries().Select(entry =>
                    new ToolStripMenuItem(entry.DisplayName, entry.Image, (sender, e) => entry.OnActivate())
                ).ToArray());
                extensionsToolStripMenuItem.DropDownItems.Add(manageExtensionsToolStripMenuItem);
                extensionsToolStripMenuItem.Enabled = extensionsToolStripMenuItem.DropDownItems.Count > 0;
            }

            private async void installUpdateMenuItem_Click(object? sender, EventArgs e)
            {
                await UpdateService.InstallUpdate();
                AppService.Exit();
            }

            private async void checkUpdatesMenuItem_Click(object? sender, EventArgs e)
            {
                _ = await UpdateService.CheckForUpdates(userRequested: true, force: true);
            }

            private void closeMenuItem_Click(object? sender, EventArgs e)
            {
                AppService.Exit();
            }

            private void manageExtensionsToolStripMenuItem_Click(object? sender, EventArgs e)
            {
                AppUI.ShowExtensionManager();
            }

            private readonly ToolStripMenuItem aboutMenuItem = new();
            private readonly ToolStripMenuItem installUpdateMenuItem = new();
            private readonly ToolStripMenuItem checkUpdatesMenuItem = new();
            private readonly ToolStripMenuItem closeMenuItem = new();
            private readonly ToolStripMenuItem settingsMenuItem = new();
            private readonly ToolStripMenuItem viewErrorsToolStripMenuItem = new();
            private readonly ToolStripMenuItem extensionsToolStripMenuItem = new();
            private readonly ToolStripMenuItem manageExtensionsToolStripMenuItem = new();
            private readonly IMenuManager MenuManager;
            private readonly IAppUI AppUI;
            private readonly IAppService AppService;
            private readonly UpdateService UpdateService;
            private readonly INotificationSink Notify;

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    Notify?.Dispose();
                }
            }

            public AppTrayMenu(
                IAppService appService,
                UpdateService updateService,
                IMenuManager menuManager,
                IAppUI appUI,
                INotificationManager notificationManager)
            {
                AppService = appService;
                UpdateService = updateService;
                MenuManager = menuManager;
                AppUI = appUI;
                Notify = new NotificationSink("apptray");
                Notify.Activated += Notify_Activated;
                notificationManager.RegisterHandler(Notify);

                // 
                // this
                // 
                this.Items.AddRange(new ToolStripItem[] {
                    this.aboutMenuItem,
                    new ToolStripSeparator(),
                    this.installUpdateMenuItem,
                    this.checkUpdatesMenuItem,
                    this.viewErrorsToolStripMenuItem,
                    this.extensionsToolStripMenuItem,
                    this.settingsMenuItem,
                    new ToolStripSeparator(),
                    this.closeMenuItem
                });
                this.Name = "appTrayMenu";
                this.Opening += new System.ComponentModel.CancelEventHandler(this.appTrayMenu_Opening);

                // 
                // aboutMenuItem
                // 
                this.aboutMenuItem.Name = "aboutMenuItem";
                this.aboutMenuItem.Size = new(170, 22);
                this.aboutMenuItem.Text = "About";
                this.aboutMenuItem.Font = new(this.aboutMenuItem.Font, FontStyle.Bold);
                this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
                // 
                // installUpdateMenuItem
                // 
                this.installUpdateMenuItem.Name = "installUpdateMenuItem";
                this.installUpdateMenuItem.Size = new(170, 22);
                this.installUpdateMenuItem.Text = "Install update";
                this.installUpdateMenuItem.Visible = false;
                this.installUpdateMenuItem.Click += new System.EventHandler(this.installUpdateMenuItem_Click);
                // 
                // checkUpdatesMenuItem
                // 
                this.checkUpdatesMenuItem.Name = "checkUpdatesMenuItem";
                this.checkUpdatesMenuItem.Size = new(170, 22);
                this.checkUpdatesMenuItem.Text = "Check for updates";
                this.checkUpdatesMenuItem.Click += new System.EventHandler(this.checkUpdatesMenuItem_Click);
                // 
                // viewErrorsToolStripMenuItem
                // 
                this.viewErrorsToolStripMenuItem.Name = "viewErrorsToolStripMenuItem";
                this.viewErrorsToolStripMenuItem.Size = new(170, 22);
                this.viewErrorsToolStripMenuItem.Text = "View errors";
                this.viewErrorsToolStripMenuItem.Click += new System.EventHandler(this.viewErrorsToolStripMenuItem_Click);
                // 
                // extensionsToolStripMenuItem
                // 
                this.extensionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    this.manageExtensionsToolStripMenuItem
                });
                this.extensionsToolStripMenuItem.Name = "extensionsToolStripMenuItem";
                this.extensionsToolStripMenuItem.Size = new(170, 22);
                this.extensionsToolStripMenuItem.Text = "Extensions";
                // 
                // manageExtensionsToolStripMenuItem
                // 
                this.manageExtensionsToolStripMenuItem.Image = FormsResources.Settings_16x;
                this.manageExtensionsToolStripMenuItem.Name = "manageExtensionsToolStripMenuItem";
                this.manageExtensionsToolStripMenuItem.Size = new(185, 22);
                this.manageExtensionsToolStripMenuItem.Text = "&Manage Extensions...";
                this.manageExtensionsToolStripMenuItem.Click += new System.EventHandler(this.manageExtensionsToolStripMenuItem_Click);
                // 
                // settingsMenuItem
                // 
                this.settingsMenuItem.Name = "settingsMenuItem";
                this.settingsMenuItem.Size = new(170, 22);
                this.settingsMenuItem.Text = "Settings";
                this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
                // 
                // closeMenuItem
                // 
                this.closeMenuItem.Name = "closeMenuItem";
                this.closeMenuItem.Size = new(170, 22);
                this.closeMenuItem.Text = "Close";
                this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            }

            private void Notify_Activated(object? sender, NotificationActivationEventArgs e)
            {
                // TODO:
            }
        }
    }
}
