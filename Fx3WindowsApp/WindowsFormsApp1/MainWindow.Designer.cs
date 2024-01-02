
namespace ImagerDebugSoftware
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadFX3FirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readFPGARevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeImageSensorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetImageClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hardwareHeatlhCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageProcessingSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageSensorSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.peripheralSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.main_propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.runningLogBox = new System.Windows.Forms.TextBox();
            this.panel_down_right = new System.Windows.Forms.Panel();
            this.button_start = new System.Windows.Forms.Button();
            this.button_pause = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.label_fail_number = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_success_number = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_left = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel_top_right = new System.Windows.Forms.Panel();
            this.button_clear = new System.Windows.Forms.Button();
            this.FOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.panel_down_right.SuspendLayout();
            this.panel_left.SuspendLayout();
            this.panel_top_right.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadFX3FirmwareToolStripMenuItem,
            this.readFPGARevisionToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // downloadFX3FirmwareToolStripMenuItem
            // 
            this.downloadFX3FirmwareToolStripMenuItem.Name = "downloadFX3FirmwareToolStripMenuItem";
            this.downloadFX3FirmwareToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.downloadFX3FirmwareToolStripMenuItem.Text = "Download FX3 Firmware";
            this.downloadFX3FirmwareToolStripMenuItem.Click += new System.EventHandler(this.downloadFX3FirmwareToolStripMenuItem_Click);
            // 
            // readFPGARevisionToolStripMenuItem
            // 
            this.readFPGARevisionToolStripMenuItem.Name = "readFPGARevisionToolStripMenuItem";
            this.readFPGARevisionToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.readFPGARevisionToolStripMenuItem.Text = "Read FPGA Revision";
            this.readFPGARevisionToolStripMenuItem.Click += new System.EventHandler(this.readFPGARevisionToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeImageSensorToolStripMenuItem,
            this.ResetImageClockToolStripMenuItem,
            this.hardwareHeatlhCheckToolStripMenuItem,
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem,
            this.videoMonitorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // initializeImageSensorToolStripMenuItem
            // 
            this.initializeImageSensorToolStripMenuItem.Name = "initializeImageSensorToolStripMenuItem";
            this.initializeImageSensorToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.initializeImageSensorToolStripMenuItem.Text = "Initialize Gigabit Transceiver";
            this.initializeImageSensorToolStripMenuItem.Click += new System.EventHandler(this.initializeImageSensorToolStripMenuItem_Click);
            // 
            // ResetImageClockToolStripMenuItem
            // 
            this.ResetImageClockToolStripMenuItem.Name = "ResetImageClockToolStripMenuItem";
            this.ResetImageClockToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.ResetImageClockToolStripMenuItem.Text = "Initialize Image Sensor";
            this.ResetImageClockToolStripMenuItem.Click += new System.EventHandler(this.ResetImageClockToolStripMenuItem_Click);
            // 
            // hardwareHeatlhCheckToolStripMenuItem
            // 
            this.hardwareHeatlhCheckToolStripMenuItem.Name = "hardwareHeatlhCheckToolStripMenuItem";
            this.hardwareHeatlhCheckToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.hardwareHeatlhCheckToolStripMenuItem.Text = "Hardware Heatlh Check";
            this.hardwareHeatlhCheckToolStripMenuItem.Click += new System.EventHandler(this.hardwareHeatlhCheckToolStripMenuItem_Click);
            // 
            // fX3BulkInterfaceLoopbackCheckToolStripMenuItem
            // 
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem.Name = "fX3BulkInterfaceLoopbackCheckToolStripMenuItem";
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem.Text = "FX3 Bulk Interface Loopback Check";
            this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem.Click += new System.EventHandler(this.fX3BulkInterfaceLoopbackCheckToolStripMenuItem_Click);
            // 
            // videoMonitorToolStripMenuItem
            // 
            this.videoMonitorToolStripMenuItem.Name = "videoMonitorToolStripMenuItem";
            this.videoMonitorToolStripMenuItem.Size = new System.Drawing.Size(259, 22);
            this.videoMonitorToolStripMenuItem.Text = "Video Monitor";
            this.videoMonitorToolStripMenuItem.Click += new System.EventHandler(this.videoMonitorToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageProcessingSettingsToolStripMenuItem,
            this.imageSensorSettingsToolStripMenuItem,
            this.peripheralSettingsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(117, 20);
            this.settingsToolStripMenuItem.Text = "Advanced Settings";
            // 
            // imageProcessingSettingsToolStripMenuItem
            // 
            this.imageProcessingSettingsToolStripMenuItem.Name = "imageProcessingSettingsToolStripMenuItem";
            this.imageProcessingSettingsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.imageProcessingSettingsToolStripMenuItem.Text = "Image Processing Settings";
            this.imageProcessingSettingsToolStripMenuItem.Click += new System.EventHandler(this.imageProcessingSettingsToolStripMenuItem_Click);
            // 
            // imageSensorSettingsToolStripMenuItem
            // 
            this.imageSensorSettingsToolStripMenuItem.Name = "imageSensorSettingsToolStripMenuItem";
            this.imageSensorSettingsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.imageSensorSettingsToolStripMenuItem.Text = "Image Acquisition Settings";
            this.imageSensorSettingsToolStripMenuItem.Click += new System.EventHandler(this.imageSensorSettingsToolStripMenuItem_Click);
            // 
            // peripheralSettingsToolStripMenuItem
            // 
            this.peripheralSettingsToolStripMenuItem.Name = "peripheralSettingsToolStripMenuItem";
            this.peripheralSettingsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.peripheralSettingsToolStripMenuItem.Text = "Peripheral Settings";
            this.peripheralSettingsToolStripMenuItem.Click += new System.EventHandler(this.peripheralSettingsToolStripMenuItem_Click);
            // 
            // main_propertyGrid
            // 
            this.main_propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.main_propertyGrid.Name = "main_propertyGrid";
            this.main_propertyGrid.Size = new System.Drawing.Size(292, 481);
            this.main_propertyGrid.TabIndex = 0;
            // 
            // runningLogBox
            // 
            this.runningLogBox.BackColor = System.Drawing.SystemColors.Info;
            this.runningLogBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runningLogBox.Location = new System.Drawing.Point(0, 0);
            this.runningLogBox.Multiline = true;
            this.runningLogBox.Name = "runningLogBox";
            this.runningLogBox.ReadOnly = true;
            this.runningLogBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.runningLogBox.Size = new System.Drawing.Size(508, 417);
            this.runningLogBox.TabIndex = 0;
            // 
            // panel_down_right
            // 
            this.panel_down_right.Controls.Add(this.button_start);
            this.panel_down_right.Controls.Add(this.button_pause);
            this.panel_down_right.Controls.Add(this.button_stop);
            this.panel_down_right.Controls.Add(this.label_fail_number);
            this.panel_down_right.Controls.Add(this.label2);
            this.panel_down_right.Controls.Add(this.label_success_number);
            this.panel_down_right.Controls.Add(this.label1);
            this.panel_down_right.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_down_right.Location = new System.Drawing.Point(292, 441);
            this.panel_down_right.Name = "panel_down_right";
            this.panel_down_right.Size = new System.Drawing.Size(508, 64);
            this.panel_down_right.TabIndex = 0;
            // 
            // button_start
            // 
            this.button_start.BackColor = System.Drawing.SystemColors.Control;
            this.button_start.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_start.Location = new System.Drawing.Point(283, 0);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 64);
            this.button_start.TabIndex = 11;
            this.button_start.Text = "Start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_pause
            // 
            this.button_pause.BackColor = System.Drawing.SystemColors.Control;
            this.button_pause.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_pause.Enabled = false;
            this.button_pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_pause.Location = new System.Drawing.Point(358, 0);
            this.button_pause.Name = "button_pause";
            this.button_pause.Size = new System.Drawing.Size(75, 64);
            this.button_pause.TabIndex = 10;
            this.button_pause.Text = "Pause";
            this.button_pause.UseVisualStyleBackColor = true;
            this.button_pause.Click += new System.EventHandler(this.button_pause_Click);
            // 
            // button_stop
            // 
            this.button_stop.BackColor = System.Drawing.SystemColors.Control;
            this.button_stop.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_stop.Enabled = false;
            this.button_stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_stop.Location = new System.Drawing.Point(433, 0);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(75, 64);
            this.button_stop.TabIndex = 9;
            this.button_stop.Text = "Stop";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // label_fail_number
            // 
            this.label_fail_number.AutoSize = true;
            this.label_fail_number.Location = new System.Drawing.Point(64, 26);
            this.label_fail_number.Name = "label_fail_number";
            this.label_fail_number.Size = new System.Drawing.Size(13, 13);
            this.label_fail_number.TabIndex = 8;
            this.label_fail_number.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Fail :";
            // 
            // label_success_number
            // 
            this.label_success_number.AutoSize = true;
            this.label_success_number.Location = new System.Drawing.Point(64, 3);
            this.label_success_number.Name = "label_success_number";
            this.label_success_number.Size = new System.Drawing.Size(13, 13);
            this.label_success_number.TabIndex = 6;
            this.label_success_number.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Success :";
            // 
            // panel_left
            // 
            this.panel_left.Controls.Add(this.main_propertyGrid);
            this.panel_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_left.Location = new System.Drawing.Point(0, 24);
            this.panel_left.Name = "panel_left";
            this.panel_left.Size = new System.Drawing.Size(292, 481);
            this.panel_left.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(292, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 481);
            this.panel2.TabIndex = 0;
            // 
            // panel_top_right
            // 
            this.panel_top_right.Controls.Add(this.button_clear);
            this.panel_top_right.Controls.Add(this.runningLogBox);
            this.panel_top_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_top_right.Location = new System.Drawing.Point(292, 24);
            this.panel_top_right.Name = "panel_top_right";
            this.panel_top_right.Size = new System.Drawing.Size(508, 417);
            this.panel_top_right.TabIndex = 1;
            // 
            // button_clear
            // 
            this.button_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_clear.FlatAppearance.BorderSize = 0;
            this.button_clear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_clear.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_clear.Location = new System.Drawing.Point(406, 388);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 8;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // FOpenDialog
            // 
            this.FOpenDialog.DefaultExt = "iic";
            this.FOpenDialog.Filter = "Intel HEX files (*.hex) | *.hex|Firmware Image files (*.iic) | *.iic";
            this.FOpenDialog.ShowReadOnly = true;
            this.FOpenDialog.Title = "Select file to download . . .";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 505);
            this.Controls.Add(this.panel_top_right);
            this.Controls.Add(this.panel_down_right);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel_left);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Imager Debug Software";
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel_down_right.ResumeLayout(false);
            this.panel_down_right.PerformLayout();
            this.panel_left.ResumeLayout(false);
            this.panel_top_right.ResumeLayout(false);
            this.panel_top_right.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox runningLogBox;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid main_propertyGrid;
        private System.Windows.Forms.ToolStripMenuItem downloadFX3FirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeImageSensorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResetImageClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hardwareHeatlhCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageProcessingSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageSensorSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem peripheralSettingsToolStripMenuItem;
        private System.Windows.Forms.Panel panel_down_right;
        private System.Windows.Forms.Panel panel_left;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel_top_right;
        private System.Windows.Forms.Label label_fail_number;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_success_number;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_pause;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.ToolStripMenuItem fX3BulkInterfaceLoopbackCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readFPGARevisionToolStripMenuItem;
        public System.Windows.Forms.OpenFileDialog FOpenDialog;
        private System.Windows.Forms.ToolStripMenuItem videoMonitorToolStripMenuItem;
    }
}

