
namespace ImagerDebugSoftware
{
    partial class VideoMonitor
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
            this.panel_image = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel_image
            // 
            this.panel_image.BackColor = System.Drawing.Color.DimGray;
            this.panel_image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_image.ForeColor = System.Drawing.Color.Black;
            this.panel_image.Location = new System.Drawing.Point(0, 0);
            this.panel_image.Name = "panel_image";
            this.panel_image.Size = new System.Drawing.Size(800, 450);
            this.panel_image.TabIndex = 3;
            // 
            // VideoMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel_image);
            this.Name = "VideoMonitor";
            this.Text = "VideoMonitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VideoMonitor_FormClosing);
            this.Load += new System.EventHandler(this.VideoMonitor_Load);
            this.SizeChanged += new System.EventHandler(this.VideoMonitor_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_image;
    }
}