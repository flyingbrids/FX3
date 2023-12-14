using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyUSB;
using ImagerDebugSoftware.PropertiesTools;
using UsbApi;
using ImagerDebugSoftware.Tasks;

namespace ImagerDebugSoftware
{
    public partial class PeripheralSettingsWindow : Form
    {
        public static PeripheralSettingsWindow AppWindow;
        public static PropertyGrid propertyGrid_peripheral;
        public static DynamicTypeDescriptor CurrentDynamicTypeDescriptor;
        public PeripheralSettingsWindow()
        {
            InitializeComponent();
            AppWindow = this;

            propertyGrid_peripheral = propertyGrid;
            InitializePropertyGrid();
        }
        public void InitializePropertyGrid()
        {
            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(PeripheralProperty));
            PeripheralProperty ppRegister = new PeripheralProperty();
            dt = dt.FromComponent(ppRegister);
            dt.PropertyChanged += PropertyChangedEventHandler;
            propertyGrid.SelectedObject = dt;
            CurrentDynamicTypeDescriptor = dt;
        }
        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {

            string propertyName = e.PropertyName;
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)sender;
            MainRegisterProperty.PropertyValueChanged(dt, propertyName);
        }

        private void UpdateRunningLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = (DateTime.Now.ToString("hh:mm:ss") + ": " + text + "\r\n\r\n ");
                MethodInvoker mi = new MethodInvoker(() =>
                {
                    textBox1.AppendText(text);
                });
                //_runningLogBox.BeginInvoke(mi);

                if (textBox1.InvokeRequired)//If in different thread
                    textBox1.BeginInvoke(mi);
                else//If in same thread
                    textBox1.AppendText(text);
                //runningLogBox.AppendText("\r\n\r\n " + DateTime.Now.ToString("hh:mm:ss") + ": " + text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateRunningLog("not implemented in hardware yet! only for demonstraion purpose!");
        }

        private void button_temp_Click(object sender, EventArgs e)
        {
            UpdateRunningLog("not implemented in hardware yet! only for demonstraion purpose!");
        }

        private void button_fan_Click(object sender, EventArgs e)
        {
            UpdateRunningLog("not implemented in hardware yet! only for demonstraion purpose!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
