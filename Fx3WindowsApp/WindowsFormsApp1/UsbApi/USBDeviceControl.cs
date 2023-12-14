using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyUSB;
using System.Threading;

namespace UsbApi
{
    public static class USBDeviceControl
    {
        private static CyFX3Device myDevice = null;
        public delegate void DelegateMsg();
        public static DelegateMsg USBAttachedEvent { get; set; }
        public static DelegateMsg USBRemovedEvent { get; set; }

        /// <summary>
        /// Initialize USB device.
        /// You must call this method at least once before you call other API.
        /// You would better call this method when your software initializei, 
        /// then the system will detect if the device is Attached or Removed automatically.
        /// </summary>
        public static void Initialize(bool isReturnImmediately = false)
        {
            USBDeviceList usbDevices = null;
            usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);


            usbDevices.DeviceAttached += new EventHandler(usbDevices_DeviceAttached);
            usbDevices.DeviceRemoved += new EventHandler(usbDevices_DeviceRemoved);
            // Get the first device having VendorID == 0x3103 and ProductID == 0x0004

            myDevice = GetUSBDevice(isReturnImmediately);

            if (usbDevices.Count > 0)
            {
                //myDevice = usbDevices[0x3103, 0x0004] as CyUSBDevice;
                //myDevice = usbDevices[0] as CyUSBDevice;
            }
        }

        private static void usbDevices_DeviceAttached(object sender, EventArgs e)
        {
            //Add code to handle Device arrival
            if (myDevice == null)
            {
                Initialize();
                USBAttachedEvent();
            }
        }

        static System.Diagnostics.Stopwatch usbDevices_DeviceRemoved_Watch = new System.Diagnostics.Stopwatch();
        private static void usbDevices_DeviceRemoved(object sender, EventArgs e)
        {
            if (!usbDevices_DeviceRemoved_Watch.IsRunning)
            {
                usbDevices_DeviceRemoved_Watch.Start();
                if (GetUSBDevice(true) == null)
                    USBRemovedEvent();
            }
            else
            {
                if (usbDevices_DeviceRemoved_Watch.ElapsedMilliseconds < 2000)
                {
                    return;
                }
                else
                {
                    usbDevices_DeviceRemoved_Watch.Reset();
                    if (GetUSBDevice(true) == null)
                        USBRemovedEvent();
                }
            }


            if (usbDevices_DeviceRemoved_Watch.ElapsedMilliseconds < 2000)
            {
                return;
            }
            else
            {
                usbDevices_DeviceRemoved_Watch.Reset();
                if (GetUSBDevice(true) == null)
                    USBRemovedEvent();
            }
            /* //Add code to handle Device removal
             if (myDevice != null)
             {
                 myDevice = null;
                 USBRemovedEvent();
             }*/
        }

        /// <summary>
        /// Get usb device instanse.
        /// Default 10 second time out counter
        /// </summary>
        /// <returns>
        /// CyUSBDevice instanse or null;
        /// </returns>
        public static CyFX3Device GetUSBDevice(bool isReturnImmediately = false)
        {
            myDevice = null;
            int timeoutCounter = 10;
            if (isReturnImmediately)
                timeoutCounter = 1;

            for (int k = 0; k < timeoutCounter; k++)
            {
                if (k > 0)
                    //Search for Boot Programmer
                    System.Threading.Thread.Sleep(1000);
                USBDeviceList usbDevices = new USBDeviceList(CyConst.DEVICES_CYUSB);
                //exit condition
                //WtimeoutCounter--;
                if (timeoutCounter < 0)
                {
                    return myDevice;
                }

                for (int i = 0; i < usbDevices.Count; i++)
                {

                    CyFX3Device tempFx3 = usbDevices[i] as CyFX3Device;
                    if ((tempFx3 != null) && (tempFx3.ProductID == 0x00f1 || tempFx3.ProductID == (ushort)243))
                    {
                        myDevice = tempFx3;
                        break;
                    }
                }
                if (myDevice != null)
                    break; // break while loop
            }
            return myDevice;
        }

        public static FX3_FWDWNLOAD_ERROR_CODE DownloadFX3Firmware(string filename)
        {
            FX3_FWDWNLOAD_ERROR_CODE enmResult = FX3_FWDWNLOAD_ERROR_CODE.FAILED;
            if (myDevice != null)
            {
                if (!myDevice.IsBootLoaderRunning())
                {
                    MessageBox.Show("Please reset FX3 to download firmware", "Bootloader is not running");
                    return enmResult;
                }
                
                enmResult = myDevice.DownloadFw(filename, FX3_FWDWNLOAD_MEDIA_TYPE.RAM);                
            }
            else
                MessageBox.Show("device not found!");
            return enmResult;
        }

        public static byte[] HexStringToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;
            byte[] arr = new byte[hexString.Length >> 1];

            for (int i = 0; i < hexString.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hexString[i << 1]) << 4) + (GetHexVal(hexString[(i << 1) + 1])));
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(arr);
            }
            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static int BulkOut(ref byte[] buf, ref int len)
        {
            int result = 2;

            if (myDevice != null && myDevice.BulkOutEndPt != null)
            {
                Boolean xferDataresult = myDevice.BulkOutEndPt.XferData(ref buf, ref len);
                result = xferDataresult ? 0 : 1;
            }

            return result;
        }

        public static bool BulkIn(ref byte[] buf, ref int len, uint timeout)
        {
            bool status = false;

            if (myDevice != null && myDevice.BulkInEndPt != null)
            {
                myDevice.BulkInEndPt.TimeOut = timeout;
                status = myDevice.BulkInEndPt.XferData(ref buf, ref len);
            }            
            return status;
        }

        public static string GetFx3Revision()
        {

            if (myDevice != null && myDevice.ControlEndPt != null)
            {
                int len = 128;
                CyControlEndPoint CtrlEndPt = myDevice.ControlEndPt;
                byte[] buf = new byte[len];
                CtrlEndPt.Target = CyConst.TGT_DEVICE;
                CtrlEndPt.ReqType = CyConst.REQ_VENDOR;
                CtrlEndPt.Direction = CyConst.DIR_FROM_DEVICE;
                CtrlEndPt.ReqCode = 0xB0;
                CtrlEndPt.Value = 0x0000;
                CtrlEndPt.Index = 0x0000;
                //CtrlEndPt.Index = (ushort)offsetAddress;
                Boolean result = CtrlEndPt.XferData(ref buf, ref len);
                if (result)
                {
                    buf = TrimEnd(buf);
                    if (BitConverter.IsLittleEndian)
                    {
                        return System.Text.Encoding.UTF8.GetString(buf);
                    }
                    else
                    {
                        Array.Reverse(buf);
                        return System.Text.Encoding.UTF8.GetString(buf);
                    }
                }
                else
                {
                    return "Invalid!";
                }
            }
            else
            {
                return "Invalid!";
            }
        }

        public static byte[] TrimEnd(byte[] array)
        {
            int lastIndex = Array.FindLastIndex(array, b => b != 0);

            Array.Resize(ref array, lastIndex + 1);

            return array;
        }

        public static Boolean WriteToRegister(byte[] buf, REGISTER offsetAddress, bool isSlave = false)
        {
            Boolean result = false;

            if (myDevice != null && myDevice.ControlEndPt != null)
            {
                int len = buf.Length;

                CyControlEndPoint CtrlEndPt = myDevice.ControlEndPt;

                byte ReqCode = 0xc2;
                if (isSlave == true)
                    ReqCode = 0xc7;

                CtrlEndPt.Target = CyConst.TGT_DEVICE;
                CtrlEndPt.ReqType = CyConst.REQ_VENDOR;
                CtrlEndPt.Direction = CyConst.DIR_TO_DEVICE;
                CtrlEndPt.ReqCode = ReqCode;                 
                CtrlEndPt.Value = 0x0000;
                CtrlEndPt.Index = (ushort)offsetAddress; //Register index / offset address

                result = CtrlEndPt.XferData(ref buf, ref len);
            }

            return result;
        }

        public static Boolean WriteImageSensor(UInt32 data = 0, UInt16 offsetAddress = 0, bool isSlave = false)
        {
            Boolean result = false;

            if (myDevice != null && myDevice.ControlEndPt != null)
            {
                int len = 1;
                byte[] buf = BitConverter.GetBytes(data);

                CyControlEndPoint CtrlEndPt = myDevice.ControlEndPt;

                byte ReqCode = 0xc4;
                if (isSlave == true)
                    ReqCode = 0xc9;
                if (offsetAddress == 0u)
                    ReqCode = 0xc1;

                CtrlEndPt.Target = CyConst.TGT_DEVICE;
                CtrlEndPt.ReqType = CyConst.REQ_VENDOR;
                CtrlEndPt.Direction = CyConst.DIR_TO_DEVICE;
                CtrlEndPt.ReqCode = ReqCode;
                CtrlEndPt.Value = 0x0000;
                CtrlEndPt.Index = (ushort)offsetAddress; 

                result = CtrlEndPt.XferData(ref buf, ref len);                
            }

            return result;
        }

        public static Boolean WriteToRegister(UInt32 data, REGISTER offsetAddress, bool isSlave = false)
        {
            Boolean result = false;

            if (myDevice != null && myDevice.ControlEndPt != null)
            {
                byte[] buf = BitConverter.GetBytes(data);
                Array.Reverse(buf);
                result = WriteToRegister(buf, offsetAddress, isSlave);
            }

            return result;
        }

        public static byte[] ReadFromRegister(REGISTER offsetAddress, bool isSlave = false)
        {

            if (myDevice != null && myDevice.ControlEndPt != null)
            {
                int len = 4;
                byte ReqCode = 0xc3;
                if (isSlave == true)
                    ReqCode = 0xc8;
                CyControlEndPoint CtrlEndPt = myDevice.ControlEndPt;
                byte[] buf = new byte[len];
                CtrlEndPt.Target = CyConst.TGT_DEVICE;
                CtrlEndPt.ReqType = CyConst.REQ_VENDOR;
                CtrlEndPt.Direction = CyConst.DIR_FROM_DEVICE;
                CtrlEndPt.ReqCode = ReqCode;                 
                CtrlEndPt.Value = 0x0000;
                CtrlEndPt.Index = (ushort)offsetAddress;
                Boolean result = CtrlEndPt.XferData(ref buf, ref len);
                if (result)
                {
                  Array.Reverse(buf);
                  return buf;  
                }
                else
                {
                    return new byte[0];
                }
            }
            else
            {
                return new byte[0];
            }
        }

        public static string ReadHexStringFromRegister(REGISTER offsetAddress, bool isSlave = false)
        {
            byte[] bytes = ReadFromRegister(offsetAddress, isSlave);
            if (bytes.Length == 0) return "Invalid!";
            if (BitConverter.ToUInt32(bytes, 0) == 0xffffffff) return "Invalid!";
            return BitConverter.ToString(bytes).Replace("-", " ");
        }

        public static UInt32 ReadUInt32FromRegister(REGISTER offsetAddress, bool isSlave = false)
        {
            byte[] bytes = ReadFromRegister(offsetAddress, isSlave);
            if (bytes.Length == 0) return 0u;
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static UInt32 ReadUInt32FromRegister(REGISTER offsetAddress, int startBitIndex, int endBitIndex, bool isSlave = false)
        {
            if (startBitIndex > endBitIndex)
            {
                throw new ArgumentException("The parameter startBitIndex can not larger than endBitIndex.");
            }
            if (startBitIndex > 31 || startBitIndex < 0)
            {
                throw new ArgumentOutOfRangeException("The range of startBitIndex is [0,31].");
            }
            if (endBitIndex > 31 || endBitIndex < 0)
            {
                throw new ArgumentOutOfRangeException("The range of endBitIndex is [0,31].");
            }

            UInt32 registerValue = ReadUInt32FromRegister(offsetAddress, isSlave);

            StringBuilder binaryString = new StringBuilder();

            while (startBitIndex > 0)
            {
                binaryString.Append("0");
                startBitIndex--;
            }

            int difference = (endBitIndex - startBitIndex);
            while (difference >= 0)
            {
                binaryString.Insert(0, "1");
                //binaryString = "1" + binaryString;
                difference--;
            }

            UInt32 binaryUInt32 = Convert.ToUInt32(binaryString.ToString(), 2);

            UInt32 result = registerValue & binaryUInt32;

            return result;
        }

        public static Boolean SetRegisterBits(UInt32 data, REGISTER offsetAddress, bool isSlave = false)
        {
            Boolean result = false;

            UInt32 Temp = ReadUInt32FromRegister(offsetAddress, isSlave);
            Temp = Temp | data;

            result = WriteToRegister(Temp, offsetAddress, isSlave);

            return result;
        }

        public static Boolean ResetRegisterBits(UInt32 data, REGISTER offsetAddress, bool isSlave = false)
        {
            Boolean result = false;            

            UInt32 Temp = ReadUInt32FromRegister(offsetAddress, isSlave);
            Temp = Temp & (~data);

            result = WriteToRegister(Temp, offsetAddress, isSlave);

            return result;
        }

        public static bool isBitSet(REGISTER offsetAddress, int bitIndex, bool isSlave = false)
        {
            UInt32 value = ReadUInt32FromRegister(offsetAddress, isSlave);
            return (((value >> bitIndex) & 1) == 1);
        }

        // APIs  
        // Get the Mode of FPGA
        public static bool IsFPGAMaster()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 0);
        }
        // Get PLL lock Status of FPGA
        public static bool IsMainPLLLocked()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 1);
        }
        public static bool IsIdle()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 5);
        }
        //Get the bit aligned status of inter FPGA transceiver
        public static bool IsInterAligned()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 3);
        }
        // Get transceiver PLL lock Status of Fx3
        public static bool IsFx3PLLLocked()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 2);
        }
        // Get GTY transceiver Status of inter-FPGA 
        public static bool IsInterFPGAGtyOK()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 4);
        }
        // Get if inter-FPGA is configured as loopback
        public static bool IsLoopBack()
        {
            return isBitSet(REGISTER.CONTROL_REG_2, 7);
        }
        // Get GTY transceiver Status of image sensor
        public static bool IsImagerGtyOK()
        {
            return isBitSet(REGISTER.CONTROL_REG_106, 0);
        }
        // Check the CRC error of SLVS_EC IP
        public static bool IsImagerCRCError()
        {
            return isBitSet(REGISTER.CONTROL_REG_106, 1);
        }
        // Check if untap image fifo overflow
        public static bool IsUntapFifoOvfl()
        {
            return isBitSet(REGISTER.CONTROL_REG_106, 28);
        }
        // Check if slave image enabled 
        public static bool IsSlaveImageEn()
        {
            return isBitSet(REGISTER.CONTROL_REG_110, 3);
        }
        // Check if raw image fifo overflow
        public static bool IsRawFifoOvfl()
        {
            return isBitSet(REGISTER.CONTROL_REG_106, 29);
        }
        // Get number of Bytes downloaded to FPGA
        public static UInt32 GetBulkOutBytesCount()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_10);
            }

            return 0;
        }
        // Get number of Bytes uploaded to FPGA
        public static UInt32 GetBulkInBytesCount()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_11);
            }

            return 0;
        }
        // reset inter-fpga transceiver status
        public static void ResetInterXceiver()
        {
            NewFrameRequest();            
            UInt32 Mask = (1u << 4);
            SetRegisterBits(Mask, REGISTER.CONTROL_REG_1);
        }

        // new frame request 
        public static void NewFrameRequest()
        {
            UInt32 Mask = (1u << 1);
            ResetRegisterBits (Mask, REGISTER.CONTROL_REG_1);
            SetRegisterBits   (Mask, REGISTER.CONTROL_REG_1);
        }

        // ROI info set 
        public static void ROIInfoSet()
        {
            UInt32 ROIid, ROIup, ROIdown, ROIleft, ROIright;
            // ROI 1
            ROIid = 31; ROIup = 168; ROIdown = 233; ROIleft = 920; ROIright= 983;
            WriteToRegister(ROIup*2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 2
            ROIid = 32; ROIup = 172; ROIdown = 237; ROIleft = 1467; ROIright = 1529;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 3
            ROIid = 60; ROIup = 534; ROIdown = 595; ROIleft = 203; ROIright = 264;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 4
            ROIid = 61; ROIup = 581; ROIdown = 642; ROIleft = 940; ROIright = 1001;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 5
            ROIid = 87; ROIup = 758; ROIdown = 816; ROIleft = 1306; ROIright = 1365;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 6
            ROIid = 117; ROIup = 1070; ROIdown = 1124; ROIleft = 1665; ROIright = 1720;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 7
            ROIid = 126; ROIup = 1199; ROIdown = 1252; ROIleft = 1007; ROIright = 1063;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 8
            ROIid = 127; ROIup = 1426; ROIdown = 1476; ROIleft = 951; ROIright = 1005;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(1, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
            // ROI 9
            ROIid = 128; ROIup = 1458; ROIdown = 1508; ROIleft = 595; ROIright = 649;
            WriteToRegister(ROIup * 2048 + ROIdown, REGISTER.CONTROL_REG_33);
            WriteToRegister(ROIleft * 4096 + ROIright, REGISTER.CONTROL_REG_34);
            WriteToRegister(ROIid, REGISTER.CONTROL_REG_35);
            WriteToRegister(3, REGISTER.CONTROL_REG_36);
            WriteToRegister(0, REGISTER.CONTROL_REG_36);
        }
        // force processor take over image sensor SPI bus
        public static void ForceImageSPI()
        {
            UInt32 Mask = (1u << 5);
            SetRegisterBits(Mask, REGISTER.CONTROL_REG_1);
            SetRegisterBits(Mask, REGISTER.CONTROL_REG_1,true);
        }
        // Let FPGA control image sensor SPI bus
        public static void ReleaseImageSPI()
        {
            UInt32 Mask = (1u << 5);
            ResetRegisterBits(Mask, REGISTER.CONTROL_REG_1);
            ResetRegisterBits(Mask, REGISTER.CONTROL_REG_1,true);
        }
        // imager clock reset
        public static bool ImageClockReset()
        {
            bool status = false;
            UInt32 Mask = 1u;
            status = ResetRegisterBits(Mask, REGISTER.CONTROL_REG_1);
            if (status == false)
                return false;
            status = SetRegisterBits(Mask, REGISTER.CONTROL_REG_1);
            if (status == false)
                return false;
            return true;
        }
        // Set Raw Image Video Mode
        public static bool VideoModeSet(bool isSlave)
        {
            bool status = false;
            UInt32 Mask = 1u;
            status = SetRegisterBits(Mask, REGISTER.CONTROL_REG_110, isSlave);
            if (status == false)
                return false;
            return true;
        }
        // Set Raw Image Trigger Mode
        public static bool VideoModeCancel(bool isSlave)
        {
            bool status = false;
            UInt32 Mask = 1u;
            status = ResetRegisterBits(Mask, REGISTER.CONTROL_REG_110, isSlave);
            if (status == false)
                return false;
            return true;
        }
        // Get LLP output mode
        public static bool GetLLPOutMode()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 0);
        }
        // Check LLP COG filter enabled
        public static bool CheckLLPIntWidthThresSel()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 2);
        }
        // Check LLP COG filter enabled
        public static bool CheckLLPCOGFilter()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 13);
        }
        // Check LLP Intensity Width filter enabled
        public static bool CheckLLPIntensityWidthFilter()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 14);
        }
        // Check LLP one COG passed
        public static bool CheckLLPOneCOGPass()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 15);
        }
        // Check LLP sobel filter enabled
        public static bool CheckLLPSobelEnabled()
        {
            return isBitSet(REGISTER.CONTROL_REG_68, 30);
        }
        // Get Sobel Edge Threshold
        public static UInt32 GetSobelEdgeThres()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_70);
            }

            return 0;
        }
        // Get Intensity Threshold
        public static UInt32 GetIntThres()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_77);
            }

            return 0;
        }
        // Get ROI Mask Info
        public static UInt32 GetROIMask()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_78);
            }

            return 0;
        }
        // Get Intensity Width Threshold
        public static UInt32 GetIntensityWidthThres()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_79);
            }

            return 0;
        }
        // Get COG filter 
        public static UInt32 GetCOGFilterRegister()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_71);
            }

            return 0;
        }
        // Get advanced COG fitler
        public static UInt32 GetAdvancedCOGFilterRegister()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_75);
            }

            return 0;
        }
        // Get pixel saturation threshold
        public static UInt32 GetPixSatThres()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_76);
            }

            return 0;
        }
        // Get # of bytes of target
        public static UInt32 GetTargetBytes()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_32);
            }

            return 0;
        }

        // Get # of bytes of cog
        public static UInt32 GetTransferBytes()
        {
            if (myDevice != null)
            {
                return ReadUInt32FromRegister(REGISTER.CONTROL_REG_5);
            }

            return 0;
        }
    }
}
