namespace Raid.Toolkit.UI.Forms
{
    partial class ExtensionsWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtensionsWindow));
			System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Active", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Errors", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup7 = new System.Windows.Forms.ListViewGroup("Disabled", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup8 = new System.Windows.Forms.ListViewGroup("Pending Uninstall", System.Windows.Forms.HorizontalAlignment.Left);
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.disableButton = new System.Windows.Forms.ToolStripButton();
			this.enableButton = new System.Windows.Forms.ToolStripButton();
			this.uninstallButton = new System.Windows.Forms.ToolStripButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.description = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "BlockActiveSkills.png");
			this.imageList.Images.SetKeyName(1, "IncreaseStamina.png");
			this.imageList.Images.SetKeyName(2, "Fear.png");
			this.imageList.Images.SetKeyName(3, "Mark.png");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.listView1);
			this.groupBox1.Controls.Add(this.toolStrip1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(248, 429);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Extensions";
			// 
			// listView1
			// 
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			listViewGroup5.CollapsedState = System.Windows.Forms.ListViewGroupCollapsedState.Expanded;
			listViewGroup5.Header = "Active";
			listViewGroup5.Name = "activeGroup";
			listViewGroup6.CollapsedState = System.Windows.Forms.ListViewGroupCollapsedState.Expanded;
			listViewGroup6.Header = "Errors";
			listViewGroup6.Name = "errorGroup";
			listViewGroup7.CollapsedState = System.Windows.Forms.ListViewGroupCollapsedState.Collapsed;
			listViewGroup7.Header = "Disabled";
			listViewGroup7.Name = "disabledGroup";
			listViewGroup8.Header = "Pending Uninstall";
			listViewGroup8.Name = "uninstallGroup";
			this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup5,
            listViewGroup6,
            listViewGroup7,
            listViewGroup8});
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(3, 44);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(242, 382);
			this.listView1.SmallImageList = this.imageList;
			this.listView1.TabIndex = 4;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.List;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// toolStrip1
			// 
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disableButton,
            this.enableButton,
            this.uninstallButton});
			this.toolStrip1.Location = new System.Drawing.Point(3, 19);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(242, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// disableButton
			// 
			this.disableButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.disableButton.Image = Properties.Resources.BlockActiveSkills;
			this.disableButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.disableButton.Name = "disableButton";
			this.disableButton.Size = new System.Drawing.Size(23, 22);
			this.disableButton.Text = "Disable";
			this.disableButton.Click += new System.EventHandler(this.disableButton_Click);
			// 
			// enableButton
			// 
			this.enableButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.enableButton.Image = Properties.Resources.ShareDamage;
			this.enableButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.enableButton.Name = "enableButton";
			this.enableButton.Size = new System.Drawing.Size(23, 22);
			this.enableButton.Text = "Enable";
			this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
			// 
			// uninstallButton
			// 
			this.uninstallButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.uninstallButton.Image = Properties.Resources.IncreaseDamageTaken;
			this.uninstallButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.uninstallButton.Name = "uninstallButton";
			this.uninstallButton.Size = new System.Drawing.Size(73, 22);
			this.uninstallButton.Text = "Uninstall";
			this.uninstallButton.Click += new System.EventHandler(this.uninstallButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.AutoSize = true;
			this.groupBox2.Controls.Add(this.description);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox2.Location = new System.Drawing.Point(248, 0);
			this.groupBox2.MinimumSize = new System.Drawing.Size(0, 150);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(531, 150);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Description";
			// 
			// description
			// 
			this.description.AutoSize = true;
			this.description.Dock = System.Windows.Forms.DockStyle.Fill;
			this.description.Location = new System.Drawing.Point(3, 19);
			this.description.Name = "description";
			this.description.Size = new System.Drawing.Size(0, 15);
			this.description.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point);
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(248, 150);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(531, 279);
			this.label1.TabIndex = 3;
			this.label1.Text = "Work in progress";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ExtensionsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(779, 429);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Icon = Properties.Resources.AppIcon;
			this.Name = "ExtensionsWindow";
			this.Text = "Manage Extensions";
			this.groupBox1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label description;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton disableButton;
        private System.Windows.Forms.ToolStripButton uninstallButton;
        private System.Windows.Forms.ToolStripButton enableButton;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
    }
}
