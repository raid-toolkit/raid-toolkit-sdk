namespace DotNetClientTest
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.apiTabList = new System.Windows.Forms.TabControl();
			this.realtimeTab = new System.Windows.Forms.TabPage();
			this.refreshView = new System.Windows.Forms.Button();
			this.viewKeyLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.eventsList = new System.Windows.Forms.ListBox();
			this.apiTabList.SuspendLayout();
			this.realtimeTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// apiTabList
			// 
			this.apiTabList.Controls.Add(this.realtimeTab);
			this.apiTabList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.apiTabList.Location = new System.Drawing.Point(0, 0);
			this.apiTabList.Name = "apiTabList";
			this.apiTabList.SelectedIndex = 0;
			this.apiTabList.Size = new System.Drawing.Size(800, 450);
			this.apiTabList.TabIndex = 0;
			// 
			// realtimeTab
			// 
			this.realtimeTab.Controls.Add(this.eventsList);
			this.realtimeTab.Controls.Add(this.label2);
			this.realtimeTab.Controls.Add(this.viewKeyLabel);
			this.realtimeTab.Controls.Add(this.refreshView);
			this.realtimeTab.Location = new System.Drawing.Point(4, 24);
			this.realtimeTab.Name = "realtimeTab";
			this.realtimeTab.Padding = new System.Windows.Forms.Padding(3);
			this.realtimeTab.Size = new System.Drawing.Size(792, 422);
			this.realtimeTab.TabIndex = 0;
			this.realtimeTab.Text = "Realtime";
			this.realtimeTab.UseVisualStyleBackColor = true;
			// 
			// refreshView
			// 
			this.refreshView.Location = new System.Drawing.Point(52, 6);
			this.refreshView.Name = "refreshView";
			this.refreshView.Size = new System.Drawing.Size(75, 23);
			this.refreshView.TabIndex = 0;
			this.refreshView.Text = "Refresh";
			this.refreshView.UseVisualStyleBackColor = true;
			this.refreshView.Click += new System.EventHandler(this.refreshView_Click);
			// 
			// viewKeyLabel
			// 
			this.viewKeyLabel.AutoSize = true;
			this.viewKeyLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.viewKeyLabel.Location = new System.Drawing.Point(133, 5);
			this.viewKeyLabel.Name = "viewKeyLabel";
			this.viewKeyLabel.Size = new System.Drawing.Size(64, 21);
			this.viewKeyLabel.TabIndex = 1;
			this.viewKeyLabel.Text = "(empty)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "View";
			// 
			// eventsList
			// 
			this.eventsList.Dock = System.Windows.Forms.DockStyle.Right;
			this.eventsList.FormattingEnabled = true;
			this.eventsList.ItemHeight = 15;
			this.eventsList.Location = new System.Drawing.Point(574, 3);
			this.eventsList.Name = "eventsList";
			this.eventsList.Size = new System.Drawing.Size(215, 416);
			this.eventsList.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.apiTabList);
			this.Name = "Form1";
			this.Text = "Form1";
			this.apiTabList.ResumeLayout(false);
			this.realtimeTab.ResumeLayout(false);
			this.realtimeTab.PerformLayout();
			this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.TabControl apiTabList;
        private System.Windows.Forms.TabPage realtimeTab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label viewKeyLabel;
        private System.Windows.Forms.Button refreshView;
        private System.Windows.Forms.ListBox eventsList;
    }
}
