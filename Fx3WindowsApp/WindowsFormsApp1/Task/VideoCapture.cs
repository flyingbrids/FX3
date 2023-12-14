using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using UsbApi;


namespace ImagerDebugSoftware.Tasks
{
    class VideoCapture
    {
        private Task _task;
        private Panel _panel_image;
        private const int img_width  = 2048;
        private const int img_height = 1088;
        private const int max_dispy_width = 1024;        
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;
        private ManualResetEvent _resetEvent;
        private byte[] raw_file_data_buffer;
        private Graphics graphic;
        private bool grpahicInUse;
        private int successCount = 0;
        private bool imageSelect;

        public VideoCapture (VideoMonitor video)
        {

            _panel_image = video.Controls["panel_image"] as Panel;

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            _resetEvent = new ManualResetEvent(true);

            _task = new Task(() =>
            {
                VideoCaptureRun();
            }
           , _token);

        }

        public void VideoCaptureStart()
        {
            if (USBDeviceControl.GetFx3Revision() == "Invalid!")
            {
                MessageBox.Show("Video Start failed! Please program Fx3!");
                return;
            }

            if (USBDeviceControl.ReadHexStringFromRegister(REGISTER.CONTROL_REG_0) == "Invalid!")
            {
                MessageBox.Show("Video Start failed! Please re-configure FPGA!");
                return;
            }

            if (USBDeviceControl.IsImagerGtyOK() == false)
            {
                MessageBox.Show("Video Start failed! Image Transceiver Error!");
                return;
            }

            if (USBDeviceControl.IsImagerCRCError() == true)
            {
                MessageBox.Show("Video Start failed! SLVS-EC CRC Error!");
                return;
            }

            imageSelect = (USBDeviceControl.IsSlaveImageEn() == true) & (USBDeviceControl.IsLoopBack() == false);            
            USBDeviceControl.NewFrameRequest();
            USBDeviceControl.WriteToRegister(0xC, REGISTER.CONTROL_REG_1, imageSelect);
          //  USBDeviceControl.WriteToRegister(0x12B21192, REGISTER.CONTROL_REG_105, imageSelect);            
            int file_bytes_send = img_width * img_height;
            int remanantCount = 1024;            
            byte[] To_file = new byte[remanantCount];
            byte[] file_send = new byte[file_bytes_send];
            for (int i = 0; i < file_bytes_send; i++)
                file_send[i] = (byte)(i & 0xff);            

            USBDeviceControl.NewFrameRequest();
            
            USBDeviceControl.VideoModeSet(imageSelect);
            USBDeviceControl.BulkIn(ref To_file, ref remanantCount, 100);
            byte[] imageSize = new byte[4];
            imageSize[3] = img_height & 0xff;
            imageSize[2] = (img_height >> 8) & 0xff;
            imageSize[1] = img_width & 0xff;
            imageSize[0] = (img_width >> 8) & 0xff;
            USBDeviceControl.WriteToRegister(imageSize, REGISTER.CONTROL_REG_3, imageSelect);
                    

            int bulkOutResult = 2;
            bulkOutResult = USBDeviceControl.BulkOut(ref file_send, ref file_bytes_send);

            if (bulkOutResult > 0)
            {
                MessageBox.Show("Video Start failed! Fx3 interface Error!");
                return;
            }

            bulkOutResult = USBDeviceControl.BulkOut(ref file_send, ref file_bytes_send);

            if (bulkOutResult > 0)
            {
                MessageBox.Show("Video Start failed! Fx3 interface Error!");
                return;
            }
            _task.Start();
        }

        public void VideoCaptureRun()
        {
            int file_pixel = img_width * img_height;
            int file_bytes = file_pixel / 2;
            int loopback_bytes = 2048;
            byte[] fileTemp1 = new byte[file_bytes];
            byte[] fileTemp2 = new byte[file_bytes];
            byte[] fileTemp3 = new byte[loopback_bytes];
            raw_file_data_buffer = new byte[file_bytes * 2];
            bool bulkInStatus = false;
            bulkInStatus = USBDeviceControl.BulkIn(ref fileTemp3, ref loopback_bytes, 1000);
            if (bulkInStatus)
            {
                while (successCount >= 0)
                {
                    _resetEvent.WaitOne();
                    if (_token.IsCancellationRequested)
                    {
                        return;
                    }
                    bulkInStatus = USBDeviceControl.BulkIn(ref fileTemp1, ref file_bytes, 3000);
                    if (bulkInStatus == false)
                    {
                        successCount--;                        
                        continue;
                    }
                    else if (successCount < 10)
                    {
                        successCount++;
                    }
                    bulkInStatus = USBDeviceControl.BulkIn(ref fileTemp2, ref file_bytes, 3000);
                    if (bulkInStatus == false)
                    {
                        successCount--;
                        continue;
                    }
                    else if (successCount < 10)
                    {
                        successCount++;
                    }

                    Array.Copy(fileTemp1, 0, raw_file_data_buffer, 0, file_bytes);
                    Array.Copy(fileTemp2, 0, raw_file_data_buffer, file_bytes, file_bytes);
                   
                    ConvertImage();                    
                }
            }

            MessageBox.Show("Video Lost! Stream Terminated!");
            FinishTask();
        }

        public void FinishTask()
        {
            if (successCount > 0)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }            
            USBDeviceControl.VideoModeCancel(imageSelect);
        }

        private unsafe void ConvertImage()
        {
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

            RenderImage(bit_map);

        }
                
        public void ClearImage ()
        {
            while (grpahicInUse) ;
            graphic.Clear(Color.DimGray);
        }

        private void RenderImage (Bitmap image)
        {
            graphic = _panel_image.CreateGraphics();            
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            int width = image.Width;
            int height = image.Height;

            int newWidth = width > max_dispy_width ? max_dispy_width : width;            
            int newHeight = height * newWidth / width;

            int pannelWidth  = _panel_image.Width;
            int pannelHeight = _panel_image.Height;
            grpahicInUse = true;
            graphic.DrawImage(image, (pannelWidth - newWidth) / 2, (pannelHeight - newHeight) / 2, newWidth, newHeight);
            image.Dispose();
            grpahicInUse = false;

        }
    
    }
}
