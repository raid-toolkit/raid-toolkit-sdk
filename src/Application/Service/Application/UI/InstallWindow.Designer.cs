
namespace Raid.Service.UI
{
	partial class InstallWindow
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.runOnStartCheckBox = new System.Windows.Forms.CheckBox();
			this.createShortcutCheckBox = new System.Windows.Forms.CheckBox();
			this.autoUpdateCheckBox = new System.Windows.Forms.CheckBox();
			this.installButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.selectInstallFolderButton = new System.Windows.Forms.Button();
			this.installationDirectory = new System.Windows.Forms.TextBox();
			this.installFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pictureBox1.Image = global::Raid.Service.Properties.Resources.RaidToolkitLarge;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(559, 239);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// runOnStartCheckBox
			// 
			this.runOnStartCheckBox.AutoSize = true;
			this.runOnStartCheckBox.Checked = true;
			this.runOnStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.runOnStartCheckBox.Location = new System.Drawing.Point(125, 41);
			this.runOnStartCheckBox.Name = "runOnStartCheckBox";
			this.runOnStartCheckBox.Size = new System.Drawing.Size(104, 19);
			this.runOnStartCheckBox.TabIndex = 1;
			this.runOnStartCheckBox.Text = "Run on startup";
			this.runOnStartCheckBox.UseVisualStyleBackColor = true;
			// 
			// createShortcutCheckBox
			// 
			this.createShortcutCheckBox.AutoSize = true;
			this.createShortcutCheckBox.Checked = true;
			this.createShortcutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.createShortcutCheckBox.Location = new System.Drawing.Point(125, 22);
			this.createShortcutCheckBox.Name = "createShortcutCheckBox";
			this.createShortcutCheckBox.Size = new System.Drawing.Size(167, 19);
			this.createShortcutCheckBox.TabIndex = 2;
			this.createShortcutCheckBox.Text = "Create start menu shortcut";
			this.createShortcutCheckBox.UseVisualStyleBackColor = true;
			// 
			// autoUpdateCheckBox
			// 
			this.autoUpdateCheckBox.AutoSize = true;
			this.autoUpdateCheckBox.Checked = true;
			this.autoUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.autoUpdateCheckBox.Location = new System.Drawing.Point(125, 3);
			this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
			this.autoUpdateCheckBox.Size = new System.Drawing.Size(197, 19);
			this.autoUpdateCheckBox.TabIndex = 3;
			this.autoUpdateCheckBox.Text = "Automatically check for updates";
			this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
			// 
			// installButton
			// 
			this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.installButton.BackColor = System.Drawing.SystemColors.Highlight;
			this.installButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.installButton.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.installButton.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.installButton.Location = new System.Drawing.Point(204, 48);
			this.installButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
			this.installButton.Name = "installButton";
			this.installButton.Size = new System.Drawing.Size(150, 40);
			this.installButton.TabIndex = 4;
			this.installButton.Text = "🚀 Install";
			this.installButton.UseVisualStyleBackColor = false;
			this.installButton.Click += new System.EventHandler(this.installButton_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.installButton, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 363);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(559, 98);
			this.tableLayoutPanel1.TabIndex = 5;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 239);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(559, 124);
			this.tableLayoutPanel2.TabIndex = 7;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.runOnStartCheckBox);
			this.panel1.Controls.Add(this.createShortcutCheckBox);
			this.panel1.Controls.Add(this.autoUpdateCheckBox);
			this.panel1.Location = new System.Drawing.Point(29, 20);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(501, 104);
			this.panel1.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "Options";
			// 
			// panel2
			// 
			this.panel2.AutoSize = true;
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.selectInstallFolderButton);
			this.panel2.Controls.Add(this.installationDirectory);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 74);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(501, 30);
			this.panel2.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Installation Directory";
			// 
			// selectInstallFolderButton
			// 
			this.selectInstallFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.selectInstallFolderButton.Location = new System.Drawing.Point(395, 3);
			this.selectInstallFolderButton.Name = "selectInstallFolderButton";
			this.selectInstallFolderButton.Size = new System.Drawing.Size(103, 23);
			this.selectInstallFolderButton.TabIndex = 1;
			this.selectInstallFolderButton.Text = "Choose folder...";
			this.selectInstallFolderButton.UseVisualStyleBackColor = true;
			this.selectInstallFolderButton.Click += new System.EventHandler(this.selectInstallFolderButton_Click);
			// 
			// installationDirectory
			// 
			this.installationDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.installationDirectory.Location = new System.Drawing.Point(125, 4);
			this.installationDirectory.Name = "installationDirectory";
			this.installationDirectory.Size = new System.Drawing.Size(264, 23);
			this.installationDirectory.TabIndex = 0;
			// 
			// InstallWindow
			// 
			this.AcceptButton = this.installButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(559, 461);
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::Raid.Service.Properties.Resources.AppIcon;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(575, 500);
			this.Name = "InstallWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Install Raid Toolkit";
			this.Load += new System.EventHandler(this.InstallWindow_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.CheckBox runOnStartCheckBox;
		private System.Windows.Forms.CheckBox createShortcutCheckBox;
		private System.Windows.Forms.CheckBox autoUpdateCheckBox;
		private System.Windows.Forms.Button installButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button selectInstallFolderButton;
		private System.Windows.Forms.TextBox installationDirectory;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.FolderBrowserDialog installFolderDialog;
	}
}