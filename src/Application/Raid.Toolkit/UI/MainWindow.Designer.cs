
namespace Raid.Toolkit.UI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.appTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.appTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.installUpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkUpdatesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.extensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.appTrayMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// appTrayIcon
			// 
			this.appTrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.appTrayIcon.ContextMenuStrip = this.appTrayMenu;
			this.appTrayIcon.Text = "Raid Toolkit";
			this.appTrayIcon.Visible = true;
			this.appTrayIcon.BalloonTipClicked += new System.EventHandler(this.appTrayIcon_BalloonTipClicked);
			this.appTrayIcon.BalloonTipClosed += new System.EventHandler(this.appTrayIcon_BalloonTipClosed);
			this.appTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.appTrayIcon_MouseClick);
			// 
			// appTrayMenu
			// 
			this.appTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installUpdateMenuItem,
            this.checkUpdatesMenuItem,
            this.viewErrorsToolStripMenuItem,
            this.extensionsToolStripMenuItem,
            this.settingsMenuItem,
            this.closeMenuItem});
			this.appTrayMenu.Name = "appTrayMenu";
			this.appTrayMenu.Size = new System.Drawing.Size(181, 158);
			this.appTrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.appTrayMenu_Opening);
			// 
			// installUpdateMenuItem
			// 
			this.installUpdateMenuItem.Name = "installUpdateMenuItem";
			this.installUpdateMenuItem.Size = new System.Drawing.Size(180, 22);
			this.installUpdateMenuItem.Text = "Install update";
			this.installUpdateMenuItem.Visible = false;
			this.installUpdateMenuItem.Click += new System.EventHandler(this.installUpdateMenuItem_Click);
			// 
			// checkUpdatesMenuItem
			// 
			this.checkUpdatesMenuItem.Name = "checkUpdatesMenuItem";
			this.checkUpdatesMenuItem.Size = new System.Drawing.Size(180, 22);
			this.checkUpdatesMenuItem.Text = "Check for updates";
			this.checkUpdatesMenuItem.Click += new System.EventHandler(this.checkUpdatesMenuItem_Click);
			// 
			// viewErrorsToolStripMenuItem
			// 
			this.viewErrorsToolStripMenuItem.Name = "viewErrorsToolStripMenuItem";
			this.viewErrorsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.viewErrorsToolStripMenuItem.Text = "View errors";
			this.viewErrorsToolStripMenuItem.Click += new System.EventHandler(this.viewErrorsToolStripMenuItem_Click);
			// 
			// settingsMenuItem
			// 
			this.settingsMenuItem.Name = "settingsMenuItem";
			this.settingsMenuItem.Size = new System.Drawing.Size(180, 22);
			this.settingsMenuItem.Text = "Settings";
			this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
			// 
			// closeMenuItem
			// 
			this.closeMenuItem.Name = "closeMenuItem";
			this.closeMenuItem.Size = new System.Drawing.Size(180, 22);
			this.closeMenuItem.Text = "Close";
			this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
			// 
			// extensionsToolStripMenuItem
			// 
			this.extensionsToolStripMenuItem.Enabled = false;
			this.extensionsToolStripMenuItem.Name = "extensionsToolStripMenuItem";
			this.extensionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.extensionsToolStripMenuItem.Text = "Extensions";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(534, 167);
			this.Icon = global::Raid.Toolkit.Properties.Resources.AppIcon;
			this.Name = "MainWindow";
			this.Text = "Raid Toolkit";
			this.Load += new System.EventHandler(this.MainWindow_Load);
			this.appTrayMenu.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon appTrayIcon;
        private System.Windows.Forms.ContextMenuStrip appTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem installUpdateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkUpdatesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extensionsToolStripMenuItem;
    }
}
