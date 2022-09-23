using Raid.Toolkit.Application.Core.Host;
using Raid.Toolkit.Application.Core.Tasks.Base;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host.Services;

namespace Raid.Toolkit.UI.Forms
{
    internal class AppTrayMenu : ContextMenuStrip
    {
        private const int kDefaultBalloonTipTimeout = 10000;

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

        private void installUpdateMenuItem_Click(object? sender, EventArgs e)
        {
            AppService.InstallUpdate();
        }

        private async void checkUpdatesMenuItem_Click(object? sender, EventArgs e)
        {
            bool hasUpdate = await UpdateService.CheckForUpdates(force: true);
            if (!hasUpdate)
            {
                AppUI.ShowNotification(
                    "No updates",
                    $"You are already running the latest version!",
                    ToolTipIcon.None,
                    kDefaultBalloonTipTimeout
                );
            }
        }

        private void closeMenuItem_Click(object? sender, EventArgs e)
        {
            AppService.Exit();
        }

        private void manageExtensionsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            AppUI.ShowExtensionManager();
        }

        private readonly ToolStripMenuItem installUpdateMenuItem = new();
        private readonly ToolStripMenuItem checkUpdatesMenuItem = new();
        private readonly ToolStripMenuItem closeMenuItem = new();
        private readonly ToolStripMenuItem settingsMenuItem = new();
        private readonly ToolStripMenuItem viewErrorsToolStripMenuItem = new();
        private readonly ToolStripMenuItem extensionsToolStripMenuItem = new();
        private readonly ToolStripMenuItem manageExtensionsToolStripMenuItem = new();
        private readonly IMenuManager MenuManager;
        private readonly IAppUI AppUI;
        private readonly AppService AppService;
        private readonly UpdateService UpdateService;

        public AppTrayMenu(
            AppService appService,
            UpdateService updateService,
            IMenuManager menuManager,
            IAppUI appUI)
        {
            AppService = appService;
            UpdateService = updateService;
            MenuManager = menuManager;
            AppUI = appUI;

            // 
            // this
            // 
            this.Items.AddRange(new ToolStripItem[] {
                this.installUpdateMenuItem,
                this.checkUpdatesMenuItem,
                this.viewErrorsToolStripMenuItem,
                this.extensionsToolStripMenuItem,
                this.settingsMenuItem,
                this.closeMenuItem
            });
            this.Name = "appTrayMenu";
            this.Opening += new System.ComponentModel.CancelEventHandler(this.appTrayMenu_Opening);

            // 
            // installUpdateMenuItem
            // 
            this.installUpdateMenuItem.Name = "installUpdateMenuItem";
            this.installUpdateMenuItem.Size = new System.Drawing.Size(170, 22);
            this.installUpdateMenuItem.Text = "Install update";
            this.installUpdateMenuItem.Visible = false;
            this.installUpdateMenuItem.Click += new System.EventHandler(this.installUpdateMenuItem_Click);
            // 
            // checkUpdatesMenuItem
            // 
            this.checkUpdatesMenuItem.Name = "checkUpdatesMenuItem";
            this.checkUpdatesMenuItem.Size = new System.Drawing.Size(170, 22);
            this.checkUpdatesMenuItem.Text = "Check for updates";
            this.checkUpdatesMenuItem.Click += new System.EventHandler(this.checkUpdatesMenuItem_Click);
            // 
            // viewErrorsToolStripMenuItem
            // 
            this.viewErrorsToolStripMenuItem.Name = "viewErrorsToolStripMenuItem";
            this.viewErrorsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.viewErrorsToolStripMenuItem.Text = "View errors";
            this.viewErrorsToolStripMenuItem.Click += new System.EventHandler(this.viewErrorsToolStripMenuItem_Click);
            // 
            // extensionsToolStripMenuItem
            // 
            this.extensionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.manageExtensionsToolStripMenuItem
            });
            this.extensionsToolStripMenuItem.Name = "extensionsToolStripMenuItem";
            this.extensionsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.extensionsToolStripMenuItem.Text = "Extensions";
            // 
            // manageExtensionsToolStripMenuItem
            // 
            this.manageExtensionsToolStripMenuItem.Image = Properties.Resources.Settings_16x;
            this.manageExtensionsToolStripMenuItem.Name = "manageExtensionsToolStripMenuItem";
            this.manageExtensionsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.manageExtensionsToolStripMenuItem.Text = "&Manage Extensions...";
            this.manageExtensionsToolStripMenuItem.Click += new System.EventHandler(this.manageExtensionsToolStripMenuItem_Click);
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(170, 22);
            this.settingsMenuItem.Text = "Settings";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(170, 22);
            this.closeMenuItem.Text = "Close";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
        }
    }
}
