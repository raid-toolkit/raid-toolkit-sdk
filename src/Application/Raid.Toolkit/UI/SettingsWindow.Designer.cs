
namespace Raid.Toolkit.UI
{
    partial class SettingsWindow
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
            this.runOnStartCheckBox = new System.Windows.Forms.CheckBox();
            this.autoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.clickToStartCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.prereleaseBuildsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // runOnStartCheckBox
            // 
            this.runOnStartCheckBox.AutoSize = true;
            this.runOnStartCheckBox.Checked = true;
            this.runOnStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.runOnStartCheckBox.Location = new System.Drawing.Point(32, 56);
            this.runOnStartCheckBox.Name = "runOnStartCheckBox";
            this.runOnStartCheckBox.Size = new System.Drawing.Size(104, 19);
            this.runOnStartCheckBox.TabIndex = 1;
            this.runOnStartCheckBox.Text = "Run on startup";
            this.runOnStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Checked = true;
            this.autoUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(32, 31);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(197, 19);
            this.autoUpdateCheckBox.TabIndex = 0;
            this.autoUpdateCheckBox.Text = "Automatically check for updates";
            this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // clickToStartCheckBox
            // 
            this.clickToStartCheckBox.AutoSize = true;
            this.clickToStartCheckBox.Checked = true;
            this.clickToStartCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clickToStartCheckBox.Location = new System.Drawing.Point(32, 81);
            this.clickToStartCheckBox.Name = "clickToStartCheckBox";
            this.clickToStartCheckBox.Size = new System.Drawing.Size(214, 19);
            this.clickToStartCheckBox.TabIndex = 2;
            this.clickToStartCheckBox.Text = "Click tray icon to launch/focus Raid";
            this.clickToStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.Location = new System.Drawing.Point(66, 161);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 10;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(147, 161);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // prereleaseBuildsCheckBox
            // 
            this.prereleaseBuildsCheckBox.AutoSize = true;
            this.prereleaseBuildsCheckBox.Checked = true;
            this.prereleaseBuildsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.prereleaseBuildsCheckBox.Location = new System.Drawing.Point(32, 106);
            this.prereleaseBuildsCheckBox.Name = "prereleaseBuildsCheckBox";
            this.prereleaseBuildsCheckBox.Size = new System.Drawing.Size(153, 19);
            this.prereleaseBuildsCheckBox.TabIndex = 3;
            this.prereleaseBuildsCheckBox.Text = "Install pre-release builds";
            this.prereleaseBuildsCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsWindow
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(288, 193);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.prereleaseBuildsCheckBox);
            this.Controls.Add(this.clickToStartCheckBox);
            this.Controls.Add(this.runOnStartCheckBox);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.Icon = global::Raid.Toolkit.Properties.Resources.AppIcon;
            this.Name = "SettingsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Raid Toolkit Settings";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox runOnStartCheckBox;
        private System.Windows.Forms.CheckBox autoUpdateCheckBox;
        private System.Windows.Forms.CheckBox clickToStartCheckBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox prereleaseBuildsCheckBox;
    }
}
