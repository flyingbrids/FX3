
namespace ImagerDebugSoftware
{
    partial class PeripheralSettingsWindow
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
            this.panel_top_right = new System.Windows.Forms.Panel();
            this.button_clear = new System.Windows.Forms.Button();
            this.runningLogBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel_left = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel_down_right = new System.Windows.Forms.Panel();
            this.button_temp = new System.Windows.Forms.Button();
            this.button_fan = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panel_top_right.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel_left.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_down_right.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_top_right
            // 
            this.panel_top_right.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_top_right.Controls.Add(this.panel1);
            this.panel_top_right.Controls.Add(this.button_clear);
            this.panel_top_right.Controls.Add(this.runningLogBox);
            this.panel_top_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_top_right.Location = new System.Drawing.Point(0, 0);
            this.panel_top_right.Name = "panel_top_right";
            this.panel_top_right.Size = new System.Drawing.Size(800, 450);
            this.panel_top_right.TabIndex = 5;
            // 
            // button_clear
            // 
            this.button_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_clear.FlatAppearance.BorderSize = 0;
            this.button_clear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_clear.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_clear.Location = new System.Drawing.Point(701, 420);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(75, 23);
            this.button_clear.TabIndex = 7;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            // 
            // runningLogBox
            // 
            this.runningLogBox.BackColor = System.Drawing.SystemColors.Info;
            this.runningLogBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runningLogBox.Location = new System.Drawing.Point(147, 65);
            this.runningLogBox.Multiline = true;
            this.runningLogBox.Name = "runningLogBox";
            this.runningLogBox.ReadOnly = true;
            this.runningLogBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.runningLogBox.Size = new System.Drawing.Size(284, 302);
            this.runningLogBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.panel_left);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(796, 446);
            this.panel1.TabIndex = 8;
            // 
            // panel_left
            // 
            this.panel_left.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_left.Controls.Add(this.propertyGrid);
            this.panel_left.Controls.Add(this.panel3);
            this.panel_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_left.Location = new System.Drawing.Point(0, 0);
            this.panel_left.Name = "panel_left";
            this.panel_left.Size = new System.Drawing.Size(320, 442);
            this.panel_left.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.panel_down_right);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(320, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(472, 442);
            this.panel2.TabIndex = 5;
            // 
            // panel_down_right
            // 
            this.panel_down_right.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_down_right.Controls.Add(this.button_temp);
            this.panel_down_right.Controls.Add(this.button_fan);
            this.panel_down_right.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_down_right.Location = new System.Drawing.Point(0, 407);
            this.panel_down_right.Name = "panel_down_right";
            this.panel_down_right.Size = new System.Drawing.Size(468, 31);
            this.panel_down_right.TabIndex = 8;
            // 
            // button_temp
            // 
            this.button_temp.BackColor = System.Drawing.SystemColors.Control;
            this.button_temp.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_temp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_temp.Location = new System.Drawing.Point(218, 0);
            this.button_temp.Name = "button_temp";
            this.button_temp.Size = new System.Drawing.Size(127, 27);
            this.button_temp.TabIndex = 0;
            this.button_temp.Text = "Read Temp Sensor";
            this.button_temp.UseVisualStyleBackColor = true;
            this.button_temp.Click += new System.EventHandler(this.button_temp_Click);
            // 
            // button_fan
            // 
            this.button_fan.BackColor = System.Drawing.SystemColors.Control;
            this.button_fan.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_fan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_fan.Location = new System.Drawing.Point(345, 0);
            this.button_fan.Name = "button_fan";
            this.button_fan.Size = new System.Drawing.Size(119, 27);
            this.button_fan.TabIndex = 2;
            this.button_fan.Text = "Read Fan Speed";
            this.button_fan.UseVisualStyleBackColor = true;
            this.button_fan.Click += new System.EventHandler(this.button_fan_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Info;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(468, 407);
            this.textBox1.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Font = new System.Drawing.Font("Microsoft YaHei", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(362, 366);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.button3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 409);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(318, 31);
            this.panel3.TabIndex = 9;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(0, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(314, 27);
            this.button3.TabIndex = 2;
            this.button3.Text = "Apply Settings";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(318, 409);
            this.propertyGrid.TabIndex = 10;
            // 
            // PeripheralSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel_top_right);
            this.Name = "PeripheralSettingsWindow";
            this.Text = "PeripheralSettingsWindow";
            this.panel_top_right.ResumeLayout(false);
            this.panel_top_right.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel_left.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_down_right.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_top_right;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel_down_right;
        private System.Windows.Forms.Button button_temp;
        private System.Windows.Forms.Button button_fan;
        private System.Windows.Forms.Panel panel_left;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.TextBox runningLogBox;
    }
}