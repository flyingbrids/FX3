using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ImagerDebugSoftware.PropertiesTools;
using static ImagerDebugSoftware.PropertiesTools.DynamicTypeDescriptor;
using UsbApi;

namespace ImagerDebugSoftware.PropertiesTools
{
    class MainRegisterProperty
    {
        
        public MainRegisterProperty()
        {
            RawImageEnable = false;
            Fx3DebugEnable = true;
            SlaveImageEnable = false;
            MLLEnable = true;
            ROIFilter = false;
            ImageWidth = 2448;
            ImageHeight = 2048;
            FrameCnt = 1;
            VMAX = 4786;
            HMAX = 4500;
            StartLine = 150;
            TrigDelay = 500;
            TrigPW = 300;
            ImageGain = 320;
        }

        #region mode
        [CustomSortedCategory("Debug Mode", 1, 5)]
        [DisplayName("FX3 Debug Enable")]
        [Description("Need to Enable if processing image sourced by Fx3 or streaming raw image from image sensor." )]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_1, MethodType = 1, ShiftBits = 3)]
        public bool Fx3DebugEnable { get; set; }

        [CustomSortedCategory("Debug Mode", 1, 5)]
        [DisplayName("Raw Image Enable")]
        [Description("If enabled, the image processing will be bypassed and the raw Image is transferred")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_1, MethodType = 1, ShiftBits = 2)]
        public bool RawImageEnable { get; set; }

        [CustomSortedCategory("Debug Mode", 1, 5)]
        [DisplayName("Slave Image Enable")]
        [Description("If enabled, the raw image is taken from the slave image sensor")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_110, MethodType = 1, ShiftBits = 3)]
        public bool SlaveImageEnable { get; set; }

        [CustomSortedCategory("Debug Mode", 1, 5)]
        [DisplayName("MLL mode")]
        [Description("If enabled, the LLP is working in MLL")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 1)]
        public bool MLLEnable { get; set; }

        [CustomSortedCategory("Debug Mode", 1, 5)]
        [DisplayName("ROI Filter Enable")]
        [Description("If enabled, the LLP is filtering out the COG within ROIs")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_1, MethodType = 1, ShiftBits = 7)]
        public bool ROIFilter { get; set; }
        #endregion mode

        #region image source
        [CustomSortedCategory("Image Source", 1, 5)]
        [DisplayName("Debug Image")]
        [Description("Select image sourced to FPGA. FPGA will NOT upload data unless the the debug image is downloaded!")]        
        [ReadOnly(false)]
        [Browsable(true)]
        [Editor(typeof(DatFileSelector), typeof(UITypeEditor))]
        public string DatFilePath { get; set; }

        [CustomSortedCategory("Image Source", 1, 5)]
        [DisplayName("Image Width")]
        [Description("Set the image width in pixel of debug image or the image from the image sensor. Min = 2048, Max = 2448 ")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_3, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 2048, Max = 2448)]
        public uint ImageWidth { get; set; }

        [CustomSortedCategory("Image Source", 1, 5)]
        [DisplayName("Image Height")]
        [Description("Set the image width in pixel of debug image or the image from the image sensor. Min = 1088, Max = 2048")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_3, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1088, Max = 2048)]
        public uint ImageHeight { get; set; }

        [CustomSortedCategory("Image Source", 1, 5)]
        [DisplayName("Frame")]
        [Description("Set number of frames to capture. Min= 1, Max = 1000")]
        [ReadOnly(false)]
        [Browsable(true)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 1000)]
        public uint FrameCnt { get; set; }
        #endregion image source

        #region image setup
        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("VMAX")]
        [Description("Please Refer to Image Sensor Datasheet, Min = 2048, Max = 5000")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_105, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 2048, Max = 5000)]
        public uint VMAX { get; set; }

        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("HMAX")]
        [Description("Please Refer to Image Sensor Datasheet, Min = 50, Max = 50000")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_105, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 50, Max = 50000)]
        public uint HMAX { get; set; }

        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("Start Line")]
        [Description("Specify the offset line. Please Refer to Image Sensor Datasheet, Min = 50, Max = 50000")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_108, MethodType = 0, ShiftBits = 0)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 50, Max = 50000)]        
        public uint StartLine { get; set; }

        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("Trigger Delay")]
        [Description("System Clk count delay from new frame request to Triggering the Imager, Min = 1, Max = 50000")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_110, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 50000)]
        public uint TrigDelay { get; set; }

        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("Trigger Pulse Width")]
        [Description("Set the H count of how long the trigger pulse stays active, Min = 1, Max = 2048")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_111, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 2048)]
        public uint TrigPW { get; set; }

        [CustomSortedCategory("Imager Setup", 1, 5)]
        [DisplayName("Image Gain")]
        [Description("Set the Image Gain in the multiple of 0.1 dB from 1 to 480")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.IMAGE_REG_GAIN, MethodType = 2, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 480)]
        public uint ImageGain { get; set; }
        #endregion image setup

        public static void PropertyValueChanged(DynamicTypeDescriptor dt, string propertyName)
        {
            if (propertyName == null) return;

            object newValue = ((DynamicProperty)dt.Properties[propertyName]).Value; // obtain new value

            object controlRegisterAttributeObject = ((DynamicProperty)dt.Properties[propertyName]).Attributes[typeof(ControlRegisterAttribute)];                      

            MainWindow.Main_PropertyGrid.Refresh();
        }

        public void WriteToRegister(TextBox runningLogBox, MainRegisterProperty registerProperty)
        {
           // MainRegisterProperty registerProperty = this.Clone();

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(MainRegisterProperty));
            dt = dt.FromComponent(registerProperty);

            PropertyDescriptorCollection properties = dt.Properties;

            for (int i = 0; i < properties.Count; i++)
            {
                DynamicProperty dynamicPropertyI = (DynamicProperty)properties[i];
                string iName = dynamicPropertyI.Name;
                string displayName = dynamicPropertyI.DisplayName;

                {
                    object value = dynamicPropertyI.Value;
                    object controlRegisterAttributeObject = dynamicPropertyI.Attributes[typeof(ControlRegisterAttribute)];


                    if (controlRegisterAttributeObject != null)
                    {
                        ControlRegisterAttribute controlRegisterAttribute = (ControlRegisterAttribute)controlRegisterAttributeObject;
                        UInt32 val;
                        if (value.GetType() == typeof(bool))
                        {
                            val = ((bool)(value)) ? 1u : 0u;
                        }
                        else
                        {
                            val = (UInt32)value;
                        }
                        
                        string message = WriteUInt32ToRegister(val, controlRegisterAttribute);
                        message = displayName + ": " + message;
                        UpdateRunningLog(message, runningLogBox);
                    }
                }
            }
        }

        public static void UpdateRunningLog(string message, TextBox _runningLogBox)
        {
            //   if (!string.IsNullOrEmpty(text))
            //   {
            //       logBox.AppendText(DateTime.Now.ToString("hh:mm:ss") + ": " + text + "\r\n\r\n");
            //   }

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

        public static string WriteUInt32ToRegister(UInt32 value, ControlRegisterAttribute controlRegisterAttribute)
        {
            REGISTER register = controlRegisterAttribute.Offset;
            int methodType = controlRegisterAttribute.MethodType;
            int shiftBits = controlRegisterAttribute.ShiftBits;
            int valueRange = controlRegisterAttribute.ValueRange;
            WriteUInt32ToRegister(value, register, methodType, shiftBits, valueRange, true);
            return WriteUInt32ToRegister(value, register, methodType, shiftBits, valueRange);
        }

        public static string WriteUInt32ToRegister(UInt32 value, REGISTER register, int methodType = 0, int shiftBits = 0, int valueRange = 32, bool isSlave = false)
        {
            if (methodType == 0)
            {
                if (valueRange < 32)
                {
                    UInt32 Temp = USBDeviceControl.ReadUInt32FromRegister(register, isSlave);
                    UInt32 mask;

                    for (int i = shiftBits; i< (shiftBits+valueRange); i++)
                    {
                        mask = 1u << i;
                        Temp = Temp & (~mask);  // reset the bit value within the range
                    }

                    value = Temp + (value << shiftBits);

                    string message = "FPGA " + ((short)register) + " Write, Value: 0x" + GetHexString(value) + ", Result: ";
                    if (USBDeviceControl.WriteToRegister(value, register, isSlave))
                    {
                        return message + "Success.";
                    }
                    else
                    {
                        return message + "Failure.";
                    }

                }

                else
                {
                    string message = "FPGA " + ((short)register) + " Write, Value: 0x" + GetHexString(value) + ", Result: ";
                    if (USBDeviceControl.WriteToRegister(value, register, isSlave))
                    {
                        return message + "Success.";
                    }
                    else
                    {
                        return message + "Failure.";
                    }
                }

            }
            else if (methodType == 1)
            {
                if (value > 0)
                {
                    value = value << shiftBits;

                    string message = "FPGA " + ((short)register) + " Set, Value: 0x" + GetHexString(value) + ", Result: ";

                    if (USBDeviceControl.SetRegisterBits(value, register, isSlave))
                    {
                        return message + "Success.";
                    }
                    else
                    {
                        return message + "Failure.";
                    }
                }
                else
                {
                    value = (1u << shiftBits);

                    string message = "FPGA " + ((short)register) + " Reset, Value: 0x" + GetHexString(value) + ", Result: ";

                    if (USBDeviceControl.ResetRegisterBits(value, register, isSlave))
                    {
                        return message + "Success.";
                    }
                    else
                    {
                        return message + "Failure.";
                    }
                }
            }

            else if (methodType == 2)
            {
                UInt16 imageRegisterOffset = (UInt16)register;
                
                string message = "Image Sensor " + ((short)register) + " Write, Value: 0x" + GetHexString(value) + ", Result: ";

                if (valueRange > 0)
                {
                    while (valueRange > 0)
                    {
                        
                        if (USBDeviceControl.WriteImageSensor((value & 0xff), imageRegisterOffset, isSlave))
                        {
                            value = value >> 8;
                            valueRange = valueRange - 8;
                            imageRegisterOffset += 0x100;
                        }
                        else
                        {
                            return message + "Failure.";
                        }                        
                    }
                    return message + "Success.";
                }

                return message + "Failure.";
            }


            else
            {
                throw new Exception("No such method: ControlRegisterAttribute.MethodType = " + methodType);
            }
        }

        public static string GetHexString(UInt32 value)
        {
            return value.ToString("X4");
        }

        public MainRegisterProperty Clone()
        {
            string initValue = this.ToString();
            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(MainRegisterProperty));
            dt = dt.FromComponent(new MainRegisterProperty());
            dt = DynamicTypeDescriptor.FromDictionaryString(dt, initValue);
            return dt.Component as MainRegisterProperty;
        }
    }
}
