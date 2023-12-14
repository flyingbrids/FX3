using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImagerDebugSoftware.PropertiesTools;
using UsbApi;

namespace ImagerDebugSoftware.Tasks
{
    class DebugCaptureTask
    {
        private TextBox _runningLogBox;
        private Button _button_start;
        private Button _button_pause;
        private Button _button_stop;
        private Label _label_success_number;
        private Label _label_fail_number;

        private Task _task;

        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private ManualResetEvent _resetEvent;
        private MainRegisterProperty _registerProperty;

        private int successCount = 0;
        private int failCount = 0;

        private int file_bytes = 0;

        private string[] input_files;
        private int filesCount;
        private string input_files_directory = " ";        

        public DebugCaptureTask (MainWindow mainWindow)
        {
            PropertyGrid main_propertyGrid = mainWindow.Controls["panel_left"].Controls["main_propertyGrid"] as PropertyGrid;
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)main_propertyGrid.SelectedObject;
            //_registerProperty = ((MainRegisterProperty)dt.Component).Clone();
            _registerProperty = ((MainRegisterProperty)dt.Component);

            _runningLogBox = mainWindow.Controls["panel_top_right"].Controls["runningLogBox"] as TextBox;
            _button_start = mainWindow.Controls["panel_down_right"].Controls["button_start"] as Button;
            _button_pause = mainWindow.Controls["panel_down_right"].Controls["button_pause"] as Button;
            _button_stop = mainWindow.Controls["panel_down_right"].Controls["button_stop"] as Button;

            _label_success_number = mainWindow.Controls["panel_down_right"].Controls["label_success_number"] as Label;
            _label_fail_number = mainWindow.Controls["panel_down_right"].Controls["label_fail_number"] as Label;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _resetEvent = new ManualResetEvent(true);

            _task = new Task(() =>
            {
                DebugCaptureRun();
            }
           , _token);
        }

        public void DebugCaptureStart()
        {
            bool status = true; //= HardwareHealthCheck();
            if (status)
            {
                _task.Start();
                UpdateRunningBox("Start Debugging...");
            }
            else
            {
                failCount++;
                UpdateRunningBox("Fail to bulk out: " + failCount);
                updateNumberLabel(successCount, failCount);
                FinishTask();
            }              
        }

        public void DebugCaptureRun()
        {
            if ((_registerProperty.RawImageEnable & (!_registerProperty.Fx3DebugEnable)) | (!_registerProperty.RawImageEnable & (_registerProperty.SlaveImageEnable)) )
            {
                MessageBox.Show("Invalid Mode!.");
                BulkOutErrorHandle();
                return;
            }           

            FileStream file = null;

            try
            {
                string selectFile = _registerProperty.DatFilePath;
                if (selectFile == null || selectFile.Length == 0)
                {
                    MessageBox.Show("Please assign a DAT file.");
                    BulkOutErrorHandle();
                    return;
                }
                filesCount = GetDataFiles(ref input_files, selectFile);                             
             }

            catch (System.IO.FileNotFoundException ex)
            {
                UpdateRunningBox("Data File Not Found: " + _registerProperty.DatFilePath);
                BulkOutErrorHandle();
                return;
            }

            string currentFile;

            for (int i = 0; i < _registerProperty.FrameCnt; i++)
            {
                currentFile = input_files[i % filesCount];

                file = new FileStream(currentFile, FileMode.Open, FileAccess.Read);

                if (file.Length != _registerProperty.ImageHeight * _registerProperty.ImageWidth *2)
                {
                    UpdateRunningBox("Incorrect Image Size! Please check if the debug image size matches the settings of image height and image width!");
                    failCount++;
                    UpdateRunningBox("Fail to bulk out: " + failCount);
                    updateNumberLabel(successCount, failCount);
                    continue;                    
                }

                _registerProperty.WriteToRegister(_runningLogBox, _registerProperty);

                // request new frame
                USBDeviceControl.NewFrameRequest();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //Pause working when _resetEvent.Reset() was called and resume working when _resetEvent.Set() was called;
                _resetEvent.WaitOne();
                //Checking if the task was canceled, if so then stop execute the rest code;
                if (_token.IsCancellationRequested)
                {
                    stopwatch.Stop();
                    return;
                }

                file_bytes = (int)file.Length;
                byte[] file_buffer = new byte[file_bytes];
                file.Read(file_buffer, 0, file_bytes);
                file.Close();

                file_bytes = file_bytes / 4;
                byte[] fileTemp1 = new byte[file_bytes];
                byte[] fileTemp2 = new byte[file_bytes];
                byte[] fileTemp3 = new byte[file_bytes];
                byte[] fileTemp4 = new byte[file_bytes];
                Array.Copy(file_buffer, 0, fileTemp1, 0, file_bytes);
                Array.Copy(file_buffer, file_bytes, fileTemp2, 0, file_bytes);
                Array.Copy(file_buffer, file_bytes * 2, fileTemp3, 0, file_bytes);
                Array.Copy(file_buffer, file_bytes * 3, fileTemp4, 0, file_bytes);

                // empty bulk in buffer    
                int remanantCount = 1024;
                byte[] To_file = new byte[remanantCount];
                USBDeviceControl.BulkIn(ref To_file, ref remanantCount, 10);

                // each bulk transfer max 4MBytes limited by driver. Split transfer for large image.
                int bulkOutResult = 2;
                bulkOutResult = USBDeviceControl.BulkOut(ref fileTemp1, ref file_bytes);
                if (bulkOutResult > 0)
                {
                    stopwatch.Stop();
                    BulkOutErrorHandle();
                    return;                    
                }
                bulkOutResult = USBDeviceControl.BulkOut(ref fileTemp2, ref file_bytes);
                if (bulkOutResult > 0)
                {
                    stopwatch.Stop();
                    BulkOutErrorHandle();
                    return;
                }
                bulkOutResult = USBDeviceControl.BulkOut(ref fileTemp3, ref file_bytes);
                if (bulkOutResult > 0)
                {
                    stopwatch.Stop();
                    BulkOutErrorHandle();
                    return;
                }
                bulkOutResult = USBDeviceControl.BulkOut(ref fileTemp4, ref file_bytes);
                if (bulkOutResult > 0)
                {
                    stopwatch.Stop();
                    BulkOutErrorHandle();
                    return;
                }
                
                // check transfer status
                if (!BulkOutDataCheck())
                {                    
                    failCount++;
                    UpdateRunningBox("Fail to bulk out: " + failCount);
                    updateNumberLabel(successCount, failCount);
                    continue;
                }

                // Bulk in loopback
                bool bulkInStatus = false;
                int loopbackCnt = 2048;
                To_file = new byte[loopbackCnt];
                bulkInStatus = USBDeviceControl.BulkIn(ref To_file, ref loopbackCnt, 10000);
                if (bulkInStatus == false)
                {
                    failCount++;
                    UpdateRunningBox("Fail to bulk In: " + failCount);
                    updateNumberLabel(successCount, failCount);
                    continue;
                }

                // Bulk in data
                string fileName = currentFile.Substring(0,currentFile.IndexOf(".dat"));

                if (_registerProperty.RawImageEnable & _registerProperty.Fx3DebugEnable) // raw image: 1 byte per pixel
                {
                    currentFile = input_files_directory + "raw_image-" + i.ToString() + ".bin";
                    bulkInStatus = USBDeviceControl.BulkIn(ref fileTemp1, ref file_bytes, 5000);
                    if (bulkInStatus == false)
                    {
                        failCount++;
                        UpdateRunningBox("Fail to bulk In: " + failCount);
                        updateNumberLabel(successCount, failCount);
                        continue;
                    }
                    bulkInStatus = USBDeviceControl.BulkIn(ref fileTemp2, ref file_bytes, 5000);
                    if (bulkInStatus == false)
                    {
                        failCount++;
                        UpdateRunningBox("Fail to bulk In: " + failCount);
                        updateNumberLabel(successCount, failCount);
                        continue;
                    }
                    if (PostDebugStatusCheck(true)) 
                    {
                        try
                        {
                            file = new FileStream(currentFile, FileMode.Create);
                            file.Write(fileTemp1, 0, file_bytes);
                            file.Write(fileTemp2, 0, file_bytes);
                            file.Close();
                        }
                        catch (Exception exc)
                        {
                            UpdateRunningBox("raw image file saved error!");
                            continue;
                        }

                        byte[] raw_file_data_buffer = new byte[file_bytes * 2];
                        Array.Copy(fileTemp1, 0, raw_file_data_buffer, 0, file_bytes);
                        Array.Copy(fileTemp2, 0, raw_file_data_buffer, file_bytes, file_bytes);

                        Convert_PNG(raw_file_data_buffer, i);
                    }

                    else
                        continue;
                }

                else
                {                    
                    // save master COG
                    currentFile = fileName + "-COGdata-" + i.ToString() + ".bin";
                    int COGCnt = (int)USBDeviceControl.GetTransferBytes() & 0xffff;
                    if (COGCnt * 4 % 512 == 0)
                    {
                        COGCnt = (int)(COGCnt * 4);
                    }
                    else
                        COGCnt = (int)((COGCnt * 4 / 512 + 1) * 512);
                    To_file = new byte[COGCnt];
                    bulkInStatus = USBDeviceControl.BulkIn(ref To_file, ref COGCnt, 10000);
                    if (bulkInStatus == false)
                    {
                        failCount++;
                        UpdateRunningBox("Fail to bulk In: " + failCount);
                        updateNumberLabel(successCount, failCount);
                        continue;
                    }
                    try
                    {
                        file = new FileStream(currentFile, FileMode.Create);
                        file.Write(To_file, 0, COGCnt);
                        file.Close();
                    }
                    catch (Exception exc)
                    {
                        UpdateRunningBox("COG data file saved error!");                        
                        continue;
                    }

                    // save master target
                    currentFile = fileName + "-Targetdata-" + i.ToString() + ".bin";
                    int targetCnt = (int)(USBDeviceControl.GetTransferBytes() >> 16);
                    if (targetCnt * 8 % 512 == 0)
                    {
                        if (targetCnt == 0)
                            targetCnt = 512;
                        else 
                            targetCnt = (int)(targetCnt * 8);
                    }
                    else
                        targetCnt = (int)((targetCnt * 8 / 512 + 1) * 512);
                    To_file = new byte[targetCnt];
                    bulkInStatus = USBDeviceControl.BulkIn(ref To_file, ref targetCnt, 10000);

                    if (bulkInStatus == false)
                    {
                        failCount++;
                        UpdateRunningBox("Fail to bulk In: " + failCount);
                        updateNumberLabel(successCount, failCount);
                        continue;
                    }
                    try
                    {
                        file = new FileStream(currentFile, FileMode.Create);
                        file.Write(To_file, 0, targetCnt);
                        file.Close();
                    }
                    catch (Exception exc)
                    {
                        UpdateRunningBox("target data file saved error!");
                        continue;
                    }
                    
                    if (USBDeviceControl.IsInterFPGAGtyOK())
                    {
                        // save slave COG 
                        currentFile = fileName + "-COGdata-Slave-" + i.ToString() + ".bin";
                        To_file = new byte[COGCnt];
                        bulkInStatus = USBDeviceControl.BulkIn(ref To_file, ref COGCnt, 10000);
                        if (bulkInStatus == false)
                        {
                            failCount++;
                            UpdateRunningBox("Fail to bulk In: " + failCount);
                            updateNumberLabel(successCount, failCount);
                            continue;
                        }
                        try
                        {
                            file = new FileStream(currentFile, FileMode.Create);
                            file.Write(To_file, 0, COGCnt);
                            file.Close();
                        }
                        catch (Exception exc)
                        {
                            UpdateRunningBox("Slave COG data file saved error!");
                            continue;
                        }

                        // save slave Target                        
                        currentFile = fileName + "-Targetdata-Slave-" + i.ToString() + ".bin";
                        To_file = new byte[targetCnt];
                        bulkInStatus = USBDeviceControl.BulkIn(ref To_file, ref targetCnt, 10000);

                        if (bulkInStatus == false)
                        {
                            failCount++;
                            UpdateRunningBox("Fail to bulk In: " + failCount);
                            updateNumberLabel(successCount, failCount);
                            continue;
                        }
                        try
                        {
                            file = new FileStream(currentFile, FileMode.Create);
                            file.Write(To_file, 0, targetCnt);
                            file.Close();
                        }
                        catch (Exception exc)
                        {
                            UpdateRunningBox("Slave target data file saved error!");
                            continue;
                        }
                        
                    }

                }

                successCount++;
                UpdateRunningBox("Success to bulk out: " + successCount);
                updateNumberLabel(successCount, failCount);
                stopwatch.Stop();
            }
     
            FinishTask();
        }

        private int GetDataFiles (ref string[] input_files, string filename)
        {
            string[] sub_strings = filename.Split(new char[] { '\\' });
            int folder_depth = sub_strings.Length;
            for (int i = 0; i < (folder_depth - 1); i++)
                input_files_directory += sub_strings[i] + "\\";

            input_files = Directory.GetFiles(input_files_directory, "*.dat");
            Array.Sort(input_files, StringComparer.InvariantCulture);
            int number_of_files = input_files.Length;
            return number_of_files;
        }

        private void BulkOutErrorHandle()
        {
            failCount++;
            UpdateRunningBox("Bulk Out Error!" + failCount);
            updateNumberLabel(successCount, failCount);
            FinishTask();
        }

        private void FinishTask(bool success=true)
        {
            updateButtonState(true, false, false);
            updatePauseButtonText(false);

            if (!success || _token.IsCancellationRequested)
            {

                UpdateRunningBox("Mission interrupted");
            }
            else
            {
                UpdateRunningBox("Mission accomplished");
            }
        }

        private bool _isDebugPause = false;
        public void DebugCapturePause()
        {
            if (_isDebugPause)
            {
                updatePauseButtonText(false);
                _resetEvent.Set();
                _isDebugPause = false;
                UpdateRunningBox("Resume Debugging...");
            }
            else 
            {                
                updatePauseButtonText(true);
                _resetEvent.Reset();
                _isDebugPause = true;
                UpdateRunningBox("Pause Debugging...");
            }
        }

        public void DebugCaptureStop()
        {
            _tokenSource.Cancel();
            UpdateRunningBox("Stop Debugging...");

            //If the task was paused, then resume it, so the code can go on to check if the token was cancel
            if (_isDebugPause)
            {
                _resetEvent.Set();
                _isDebugPause = false;
            }           
            _tokenSource.Dispose();
            FinishTask(false);
        }

        private void updateNumberLabel(int successNumber, int falilNumber)
        {
            MethodInvoker mi = new MethodInvoker(() =>
            {
                _label_success_number.Text = successNumber.ToString();
                _label_fail_number.Text = falilNumber.ToString();
            });

            if (_label_success_number.InvokeRequired)//If in different thread
            {
                _label_success_number.BeginInvoke(mi);
            }
            else//If in same thread
            {
                _label_success_number.Text = successNumber.ToString();
                _label_fail_number.Text = falilNumber.ToString();
            }
        }

        private void updateButtonState(bool start, bool pause, bool stop)
        {
            MethodInvoker mStart = new MethodInvoker(() =>
            {
                _button_start.Enabled = start;
                _button_pause.Enabled = pause;
                _button_stop.Enabled = stop;
            });

            if (_button_start.InvokeRequired)//If in different thread
            {
                _button_pause.BeginInvoke(mStart);
            }
            else//If in same thread
            {
                _button_start.Enabled = start;
                _button_pause.Enabled = pause;
                _button_stop.Enabled = stop;
            }
        }

        private void updatePauseButtonText(bool isPaused)
        {
            MethodInvoker mi = new MethodInvoker(() =>
            {
                _button_pause.Text = isPaused ? "Resume" : "Pause";
            });

            if (_button_pause.InvokeRequired)//If in different thread
            {
                _button_pause.BeginInvoke(mi);
            }
            else//If in same thread
            {
                _button_pause.Text = isPaused ? "Resume" : "Pause";
            }
        }

        public bool HardwareHealthCheck()    
        {
            string Revision = USBDeviceControl.GetFx3Revision();
            if (Revision == "Invalid!")
            {
                UpdateRunningBox("Failed! Invalid Fx3 firmware! Did you forget to download the Fx3 Firmware?" +
                                 " Please go to file->download Fx3 firmware to download the proper firmware!");
                return false;
            }            

            Revision = USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0);
            if (Revision == "Invalid!")
            {
                UpdateRunningBox("Failed! Invalid FPGA revision! Did you forget to configure the FPGA?");
                return false;
            }

            bool status = USBDeviceControl.IsFPGAMaster();
            if (status == false)
            {
                UpdateRunningBox("Failed! The FPGA connected isn't configured as Master");
                return false;
            }
            status = USBDeviceControl.IsMainPLLLocked();
            if (status == false)
            {
                UpdateRunningBox("Failed! The Main PLL isn't locked");
                return false;
            }

            status = USBDeviceControl.IsFx3PLLLocked();
            if (status == false)
            {
                UpdateRunningBox("Failed! The Fx3 PLL isn't locked");
                return false;
            }
            /*
            status = USBDeviceControl.IsInterFPGAGtyOK();
            if (status == false)
            {
                UpdateRunningBox("Failed! The GTY transceiver for Inter-FPGA communication is not running properly!");
                return false;
            }
            */
            
            status = USBDeviceControl.IsImagerGtyOK();
            if (status == false)
            {
                UpdateRunningBox("Failed! Error reported by Image Sensor transceiver. Did you forget to initialize the image sensor?" +
                                 " Please go to tool->initialize the image sensor or reset image sensor clock!");
                return false;
            }
            
            status = USBDeviceControl.IsImagerCRCError();
            if (status == true)
            {
                UpdateRunningBox("Failed! CRC Error reported by SLVS_EC IP. Check CRC settings in both image sensor and SLVS_EC IP!");
                return false;
            }

            status = USBDeviceControl.IsIdle();
            if (status == false)
            {
                UpdateRunningBox("Please wait till inter-FPGA interface to be idle!");
                return false;
            }

            UpdateRunningBox("Hardware Health Check Passed!");
            return true;
        }

        public bool BulkOutDataCheck()
        {
            UInt32 imageBytes = _registerProperty.ImageHeight * _registerProperty.ImageWidth *2;
            return (imageBytes == USBDeviceControl.GetBulkOutBytesCount());
        }

        public bool PostDebugStatusCheck(bool rawMode)
        {
            bool status;
            if (rawMode)
            {
                status = USBDeviceControl.IsRawFifoOvfl();
                if (status)
                {
                    UpdateRunningBox("Error! The image raw FIFO overflowed! Need to slow down the image streaming!");
                    return false;
                }
            }
            else
            {
                status = USBDeviceControl.IsUntapFifoOvfl();
                if (status)
                {
                    UpdateRunningBox("Error! The image untap FIFO overflowed! Need to slow down the image streaming!");
                    return false;
                }
            }

            UpdateRunningBox("Successfully transferred or processed the image from the image sensor");
            return true;
        }

        private void UpdateRunningBox(string message)
        {

            message = (DateTime.Now.ToString("hh:mm:ss") + ": " + message + "\r\n\r\n");
            MethodInvoker mi = new MethodInvoker(() =>
            {
                _runningLogBox.AppendText(message);
            });
            //_runningLogBox.BeginInvoke(mi);

            if (_runningLogBox.InvokeRequired)//If in different thread
                _runningLogBox.BeginInvoke(mi);
            else//If in same thread
                _runningLogBox.AppendText(message);
        }

        private unsafe void Convert_PNG(byte[] raw_file_data_buffer, int index)
        {            
                
                int img_width  = (int) _registerProperty.ImageWidth;
                int img_height = (int) _registerProperty.ImageHeight;               

                // Save the BMP file
                Rectangle rectangle = new Rectangle(0, 0, img_width, img_height);
                Bitmap bit_map = new Bitmap(img_width, img_height, PixelFormat.Format24bppRgb);
                BitmapData bit_map_data = bit_map.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                int offset = bit_map_data.Stride - rectangle.Width * 3;
                byte* p = (byte*)bit_map_data.Scan0.ToPointer();

                for (int row = 0; row < img_height; row++)
                {
                    for (int column = 0; column < img_width; column++)
                    {
                        p[0] = raw_file_data_buffer[row * img_width + column];
                        p[1] = raw_file_data_buffer[row * img_width + column];
                        p[2] = raw_file_data_buffer[row * img_width + column];
                        p += 3;
                    }
                    p += offset;
                }

                bit_map.UnlockBits(bit_map_data);

                // Save the bitmap (to a *.png file)
                
                string bmpFileName = "C:\\" + input_files_directory.Substring(4) + "raw" + index.ToString() + ".png";
                bit_map.Save(bmpFileName, ImageFormat.Png);
           
        }

    }
}
