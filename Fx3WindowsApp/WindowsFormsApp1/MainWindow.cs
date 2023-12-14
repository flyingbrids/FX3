using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
    public partial class MainWindow : Form
    {
        public static MainWindow AppWindow;
        public static PropertyGrid Main_PropertyGrid;
        public static DynamicTypeDescriptor CurrentDynamicTypeDescriptor;
        private DebugCaptureTask DebugCaptureTask;
        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;

            Main_PropertyGrid = main_propertyGrid;
            InitializePropertyGrid();

        }

        public void InitializePropertyGrid()
        {
            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(MainRegisterProperty));
            MainRegisterProperty mainRegister = new MainRegisterProperty();
            dt = dt.FromComponent(mainRegister);            
            dt.PropertyChanged += PropertyChangedEventHandler;
            Main_PropertyGrid.SelectedObject = dt;
            CurrentDynamicTypeDescriptor = dt;
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {

            string propertyName = e.PropertyName;
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)sender;
            MainRegisterProperty.PropertyValueChanged(dt, propertyName);
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            #region Initialize FX3
            USBDeviceControl.USBAttachedEvent += usbDevices_DeviceAttached;
            USBDeviceControl.USBRemovedEvent += usbDevices_DeviceRemoved;
            USBDeviceControl.Initialize(true);
            CheckUSBDeviceStatus(true);
            #endregion Initialize FX3
        }
        
        private void usbDevices_DeviceRemoved()
        {
            //Add code to handle Device removal
            //myDevice = null;
            UpdateRunningLog("Dev Kit has been Removed");
        }

        private long lastDeviceAttachedTime = 0;
        private void usbDevices_DeviceAttached()
        {
            //Add code to handle Device arrival
            //Initialize();

            if (lastDeviceAttachedTime == 0)
            {
                lastDeviceAttachedTime = Convert.ToInt64((DateTime.Now - new DateTime(2020, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            }
            else
            {
                long nowTime = Convert.ToInt64((DateTime.Now - new DateTime(2020, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                if ((nowTime - lastDeviceAttachedTime) < 3)
                {
                    return;
                }
                else
                {
                    lastDeviceAttachedTime = nowTime;
                }
            }

            UpdateRunningLog("Dev Kit has been attached");
            CheckUSBDeviceStatus();
        }

        private void CheckUSBDeviceStatus(bool isReturnImmediately = false)
        {
            CyFX3Device fx3 = USBDeviceControl.GetUSBDevice(isReturnImmediately);
            if (fx3 != null)
            {
                UpdateRunningLog("Dev Kit is ready: " + fx3.FriendlyName);                               
                UpdateRunningLog("FX3 Revision is: " + USBDeviceControl.GetFx3Revision());
            }

            else
            {
                UpdateRunningLog("Dev Kit is not available. Please check power and cable.");
            }
        }

        private void UpdateRunningLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = (DateTime.Now.ToString("hh:mm:ss") + ": " + text + "\r\n\r\n ");
                MethodInvoker mi = new MethodInvoker(() =>
                {
                    runningLogBox.AppendText(text);
                });
                //_runningLogBox.BeginInvoke(mi);

                if (runningLogBox.InvokeRequired)//If in different thread
                    runningLogBox.BeginInvoke(mi);
                else//If in same thread
                    runningLogBox.AppendText(text);
                //runningLogBox.AppendText("\r\n\r\n " + DateTime.Now.ToString("hh:mm:ss") + ": " + text);
            }
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            runningLogBox.Text = "";
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            button_start.Enabled = false;
            button_pause.Enabled = true;
            button_stop.Enabled = true;
            DebugCaptureTask = new DebugCaptureTask(this);
            DebugCaptureTask.DebugCaptureStart();            
        }

        private void button_pause_Click(object sender, EventArgs e)
        {
            DebugCaptureTask.DebugCapturePause();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            DebugCaptureTask.DebugCaptureStop();
        }

        private void readFPGARevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
           UpdateRunningLog("FPGA Revision is: " + USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0));
        }

        private void hardwareHeatlhCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebugCaptureTask = new DebugCaptureTask(this);
            DebugCaptureTask.HardwareHealthCheck();
        }

        private void initializeImageSensorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (USBDeviceControl.GetFx3Revision() == "Invalid!")
            {
                MessageBox.Show("Failed! Please program Fx3!");
                return;
            }

            if (USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0) == "Invalid!")
            {
                MessageBox.Show("Failed! Please re-configure FPGA!");
                return;
            }

            // initialize inter FPGA communication
            USBDeviceControl.ResetInterXceiver();
            Thread.Sleep(200);
            if (USBDeviceControl.IsInterAligned() == false) 
            {
                MessageBox.Show("Failed! Inter FPGA GTY transceiver not aligned!");
                return;
            }  
            
            USBDeviceControl.NewFrameRequest();

            if (USBDeviceControl.IsInterFPGAGtyOK()) 
            {
                // TODO: send and receive data to slave to make sure good communication is established.

                MessageBox.Show("Inter FPGA transceiver initialized successfully!");
            }
            else
                MessageBox.Show("Inter FPGA transceiver initialized failed!");

       }

        private void ResetImageClockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (USBDeviceControl.GetFx3Revision() == "Invalid!")
            {
                MessageBox.Show("Failed! Please program Fx3!");
                return;
            }

            if (USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0) == "Invalid!")
            {
                MessageBox.Show("Failed! Please re-configure FPGA!");
                return;
            }

            // force processor take over SPI bus to image sensor
            USBDeviceControl.ForceImageSPI();
            // initialize image sensor
            if (USBDeviceControl.WriteImageSensor())
            {
                Thread.Sleep(4000);
                USBDeviceControl.NewFrameRequest();
                Thread.Sleep(2000);
                // let FPGA take SPI bus to image sensor
                 USBDeviceControl.ReleaseImageSPI();
                // check image status
                if (USBDeviceControl.IsImagerGtyOK())
                {
                    MessageBox.Show("Image sensor is initialized successfully!");
                    return;
                }
                MessageBox.Show("Image sensor initialized failed!");
            }

            //if (USBDeviceControl.ImageClockReset())
            //    MessageBox.Show("Image clock is reset successfully!");
           // else
            //    MessageBox.Show("Image clock failed to reset!");
        }

        private void fX3BulkInterfaceLoopbackCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            if (USBDeviceControl.GetFx3Revision() == "Invalid!")
            {
                MessageBox.Show("Fx3 Bulk interface loopback check failed! Please program Fx3!");
                return;
            }

            if (USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0) == "Invalid!")
            {
                MessageBox.Show("Fx3 Bulk interface loopback check failed! Please re-configure FPGA!");
                return;
            }

            USBDeviceControl.NewFrameRequest();
            int file_bytes_send = 8192; // fill up bulk in buffer
            int file_bytes_receive = 2048; // loop back 2048 bytes and check 
            bool xferStatus = false;
            USBDeviceControl.WriteToRegister(0x08000002, REGISTER.CONTROL_REG_3);
            USBDeviceControl.NewFrameRequest();

            int remanantCount = 1024;
            byte[] To_file = new byte[remanantCount];

            byte[] file_send = new byte[file_bytes_send];
            byte[] file_compare = new byte[file_bytes_receive];
            byte[] file_receive = new byte[file_bytes_receive];

            for (int i = 0; i < file_bytes_send; i++)
                file_send[i] = (byte)(i & 0xff);

            Array.Copy(file_send, file_compare, file_bytes_receive);

            int bulkOutResult = 2;

            USBDeviceControl.BulkIn(ref To_file, ref remanantCount, 10);

            Thread.Sleep(1);
                
            bulkOutResult = USBDeviceControl.BulkOut(ref file_send, ref file_bytes_send);
            if (bulkOutResult > 0)
            {
                MessageBox.Show("Fx3 Bulk interface loopback check failed!");
                return;
            }

            xferStatus = USBDeviceControl.BulkIn(ref file_receive, ref file_bytes_receive, 5000);
            if (xferStatus == false)
            {
                MessageBox.Show("Fx3 Bulk interface loopback check failed!");
                return;
            }
           
           for (int i=0; i< file_bytes_receive; i++)
            {
                if (file_receive[i]!= file_compare[i])
                {
                    MessageBox.Show("Fx3 Bulk interface loopback check failed!");
                    return;
                }
            }           

            MessageBox.Show("Fx3 Bulk interface loopback check pass!");
        }

        private void downloadFX3FirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FOpenDialog.Filter = "Firmware Image files (*.img) | *.img";
            if (FOpenDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = FOpenDialog.FileName;
                if (USBDeviceControl.DownloadFX3Firmware(filename) == FX3_FWDWNLOAD_ERROR_CODE.SUCCESS)
                    MessageBox.Show("Fx3 Firmware Downloaded Successfully");
            }
            FOpenDialog.FileName = "";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© 2022 FARO Technology. All Rights Reserved.");
        }

        private void imageProcessingSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Revision = USBDeviceControl.GetFx3Revision();
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid Fx3 firmware! Did you forget to download the Fx3 Firmware?");
                return;
            }

            Revision = USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0);
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid FPGA revision! Did you forget to configure the FPGA?");
                return;
            }

            ImageProcessingWindow IPSettings = new ImageProcessingWindow();
            IPSettings.ShowDialog();
        }

        private void contentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var process = Process.Start("msedge.exe",
            "https://faroinc.sharepoint.com/:w:/r/sites/EVEHandheldscannerformetrology/_layouts/15/Doc.aspx?sourcedoc=%7BDAAE596D-E3E1-4682-8FE1-629EAF5E77B5%7D&file=Eve%20Imager%20Design%20Based%20on%20ADS9%20Kit.docx&action=default&mobileredirect=true");            
            process.Start();
        }

        private void imageSensorSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Revision = USBDeviceControl.GetFx3Revision();
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid Fx3 firmware! Did you forget to download the Fx3 Firmware?");
                return;
            }

            Revision = USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0);
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid FPGA revision! Did you forget to configure the FPGA?");
                return;
            }

            ImageAcquisitonWindow IASettings = new ImageAcquisitonWindow();
            IASettings.ShowDialog();
        }

        private void peripheralSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Revision = USBDeviceControl.GetFx3Revision();
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid Fx3 firmware! Did you forget to download the Fx3 Firmware?");
                return;
            }

            Revision = USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0);
            if (Revision == "Invalid!")
            {
                MessageBox.Show("Failed! Invalid FPGA revision! Did you forget to configure the FPGA?");
                return;
            }

            PeripheralSettingsWindow PPSettings = new PeripheralSettingsWindow();
            PPSettings.ShowDialog();
        }

        private void videoMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (USBDeviceControl.IsIdle())
            {
                VideoMonitor ImageVideo = new VideoMonitor();
                ImageVideo.ShowDialog();
            }
            else
            {
                MessageBox.Show("Failed! Please wait till inter FPGA transfer to be idle!");
                return;
            }

        }
    }

}
