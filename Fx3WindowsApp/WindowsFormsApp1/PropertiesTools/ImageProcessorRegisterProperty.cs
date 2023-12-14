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
    class ImageProcessorRegisterProperty
    {
        #region LLP
        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Peak Filter Bypass")]
        [Description("Bypass or use Peak Filter ")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 30)]
        public bool PeakFilterEnable { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("One COG pass")]
        [Description("Pass the solo COG in a column")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 15)]
        public bool OneCOGPass { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Intensity Width Filter Disable")]
        [Description("Enable or disable Intensity Width Filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 14)]
        public bool IntensityWidthFilterEnable { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Enable COG Filter")]
        [Description("Enable or disable COG Filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 13)]
        public bool COGFilterEnable { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Intensity Width Threshold Enable")]
        [Description("Enable Intentisy Width Threashold sent by software. If disable, FPGA will use the value calcualted by intensity filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 2)]
        public bool IntWidthThresEnable { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Multi-COG Output Enable")]
        [Description("True will output Multi-COG. False will output single COG")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_68, MethodType = 1, ShiftBits = 0)]
        public bool MultCOGEnable { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Peak Filter Edge Positive Threshold")]
        [Description("Positive value threshold for peak filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_70, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 2000)]
        public UInt32 PeakPosThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Peak Filter Edge Negative Threshold")]
        [Description("Negative value threshold for peak filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_70, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 5000)]
        public UInt32 PeakNegThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Minimum COG Width")]
        [Description("Minimum COG Width for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_71, MethodType = 0, ShiftBits = 24, ValueRange = 6)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 63)]
        public UInt32 MinCOGWidth { get; set;}

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Minimum COG Pixel Intensity")]
        [Description("Minimum COG Pixel Intensity for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_71, MethodType = 0, ShiftBits = 12, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 MinCOGPix { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Maximum COG Pixel Intensity")]
        [Description("Maximum COG Pixel Intensity for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_71, MethodType = 0, ShiftBits = 0, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 MaxCOGPix { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Flatness Threshold")]
        [Description("Flatness Threshold for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_75, MethodType = 0, ShiftBits = 20, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 FlatThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Min Max Threshold")]
        [Description("Min Max Threshold for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_75, MethodType = 0, ShiftBits = 10, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 MinMaxThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("First Last Threshold")]
        [Description("First Last Threshold for COG filter")]
        [ReadOnly(false)]
        [Browsable(false)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_75, MethodType = 0, ShiftBits = 0, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 FirstLastThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Pixel Saturation Threshold")]
        [Description("Pixel Saturation Threshold")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_76, MethodType = 0, ShiftBits = 0, ValueRange = 10)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 1023)]
        public UInt32 PixelSatThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Intensisty Positive Threshold")]
        [Description("Intensisty Positive Threshold")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_77, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 500)]
        public UInt32 IntPosThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Intensisty Negative Threshold")]
        [Description("Intensisty Negative Threshold")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_77, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 500)]
        public UInt32 IntNegThres { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Max ROI Mask Width")]
        [Description("Maximum ROI Mask Width")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_78, MethodType = 0, ShiftBits = 0, ValueRange = 6)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 63)]
        public UInt32 MaxMaskWidth { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Min ROI Mask Width")]
        [Description("Minimum ROI Mask Width")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_78, MethodType = 0, ShiftBits = 8, ValueRange = 6)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 63)]
        public UInt32 MinMaskWidth { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("ROI Mask Width Coefficient")]
        [Description("ROI Mask Width Coefficient for multiplication. Format 1.7")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_78, MethodType = 0, ShiftBits = 16, ValueRange = 8)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 255)]
        public UInt32 MaskCoeff { get; set; }

        [CustomSortedCategory("Laser Line Probe", 1, 5)]
        [DisplayName("Intensity Width Threshold")]
        [Description("Intensity Width Threshold")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_79, MethodType = 0, ShiftBits = 16, ValueRange = 6)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 0, Max = 63)]
        public UInt32 IntWidthThres { get; set; }
        #endregion LLP

        #region Target
        [CustomSortedCategory("Target ROI Processing", 1, 5)]
        [DisplayName("Upper Bound of Connected Point")]
        [Description("Set the Upper Bound of band pass connect point filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_39, MethodType = 0, ShiftBits = 16, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 512)]
        public UInt32 highPointLim { get; set; }

        [CustomSortedCategory("Target ROI Processing", 1, 5)]
        [DisplayName("Lower Bound of Connected Point")]
        [Description("Set the Lower Bound of band pass connect point filter")]
        [ReadOnly(false)]
        [Browsable(true)]
        [ControlRegister(Offset = REGISTER.CONTROL_REG_39, MethodType = 0, ShiftBits = 0, ValueRange = 16)]
        [TypeConverter(typeof(CustomUInt32Converter))]
        [UInt32Validation(Min = 1, Max = 512)]
        public UInt32 lowPointLim { get; set; }
        #endregion Target

        public ImageProcessorRegisterProperty()
        {
            #region LLP
            MultCOGEnable = USBDeviceControl.GetLLPOutMode();
            COGFilterEnable = USBDeviceControl.CheckLLPCOGFilter();
            OneCOGPass = USBDeviceControl.CheckLLPOneCOGPass();
            IntensityWidthFilterEnable = USBDeviceControl.CheckLLPIntensityWidthFilter();
            IntWidthThresEnable = USBDeviceControl.CheckLLPIntWidthThresSel();
            PeakFilterEnable = USBDeviceControl.CheckLLPSobelEnabled();
            PeakPosThres = (USBDeviceControl.GetSobelEdgeThres()) & (0xffff);
            PeakNegThres = (~(USBDeviceControl.GetSobelEdgeThres() >> 16) + 1) & (0xffff);
            MinCOGWidth = (USBDeviceControl.GetCOGFilterRegister() >> 24) & (0x3f);
            MinCOGPix = (USBDeviceControl.GetCOGFilterRegister() >> 12) & (0x3ff);
            MaxCOGPix = (USBDeviceControl.GetCOGFilterRegister()) & (0x3ff);
            FlatThres = (USBDeviceControl.GetAdvancedCOGFilterRegister() >> 20) & (0x3ff);
            MinMaxThres = (USBDeviceControl.GetAdvancedCOGFilterRegister() >> 10) & (0x3ff);
            FirstLastThres = (USBDeviceControl.GetAdvancedCOGFilterRegister()) & (0x3ff);
            PixelSatThres = (USBDeviceControl.GetPixSatThres()) & (0x3ff);
            IntPosThres = (USBDeviceControl.GetIntThres()) & (0xffff);
            IntNegThres = (USBDeviceControl.GetIntThres() >> 16) & (0xffff);
            MaxMaskWidth = (USBDeviceControl.GetROIMask() & 0x3f);
            MinMaskWidth = ((USBDeviceControl.GetROIMask() >> 8) & 0x3f);
            MaskCoeff = ((USBDeviceControl.GetROIMask() >> 16) & 0xff);
            IntWidthThres = (USBDeviceControl.GetIntensityWidthThres() & 0x7f);
            #endregion LLP
            highPointLim = 230;
            lowPointLim = 20;
        }

        public static void PropertyValueChanged(DynamicTypeDescriptor dt, string propertyName)
        {
            if (propertyName == null) return;

            object newValue = ((DynamicProperty)dt.Properties[propertyName]).Value;

            if (propertyName == "COGFilterEnable")
            {
                bool value = (bool)newValue;
                DynamicProperty dp1 = (DynamicProperty)dt.Properties["MinCOGWidth"];
                DynamicProperty dp2 = (DynamicProperty)dt.Properties["MinCOGPix"];
                DynamicProperty dp3 = (DynamicProperty)dt.Properties["MaxCOGPix"];
                DynamicProperty dp4 = (DynamicProperty)dt.Properties["FlatThres"];
                DynamicProperty dp5 = (DynamicProperty)dt.Properties["MinMaxThres"];
                DynamicProperty dp6 = (DynamicProperty)dt.Properties["FirstLastThres"];
                if (value)
                {
                    dp1.SetBrowsable(true);
                    dp2.SetBrowsable(true);
                    dp3.SetBrowsable(true);
                    dp4.SetBrowsable(true);
                    dp5.SetBrowsable(true);
                    dp6.SetBrowsable(true);
                }
                else
                {
                    dp1.SetBrowsable(false);
                    dp2.SetBrowsable(false);
                    dp3.SetBrowsable(false);
                    dp4.SetBrowsable(false);
                    dp5.SetBrowsable(false);
                    dp6.SetBrowsable(false);
                }
            }

            ImageProcessingWindow.IP_PropertyGrid.Refresh();
        }

        public void WriteToRegister(ImageProcessorRegisterProperty registerProperty)
        {
            // MainRegisterProperty registerProperty = this.Clone();

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(ImageProcessorRegisterProperty));
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
                        
                    }
                }
            }
        }

        public static string WriteUInt32ToRegister(UInt32 value, ControlRegisterAttribute controlRegisterAttribute)
        {
            REGISTER register = controlRegisterAttribute.Offset;
            int methodType = controlRegisterAttribute.MethodType;
            int shiftBits = controlRegisterAttribute.ShiftBits;
            int valueRange = controlRegisterAttribute.ValueRange;
            return WriteUInt32ToRegister(value, register, methodType, shiftBits, valueRange);
        }

        public static string WriteUInt32ToRegister(UInt32 value, REGISTER register, int methodType = 0, int shiftBits = 0, int valueRange = 32)
        {
            if (methodType == 0)
            {
                if (valueRange < 32)
                {
                    UInt32 Temp = USBDeviceControl.ReadUInt32FromRegister(register);
                    UInt32 mask;

                    for (int i = shiftBits; i < (shiftBits + valueRange); i++)
                    {
                        mask = 1u << i;
                        Temp = Temp & (~mask);  // reset the bit value within the range
                    }

                    value = Temp + (value << shiftBits);

                    string message = "#" + ((short)register) + " Write, Value: 0x" + GetHexString(value) + ", Result: ";
                    if (USBDeviceControl.WriteToRegister(value, register))
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
                    string message = "#" + ((short)register) + " Write, Value: 0x" + GetHexString(value) + ", Result: ";
                    if (USBDeviceControl.WriteToRegister(value, register))
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

                    string message = "#" + ((short)register) + " Set, Value: 0x" + GetHexString(value) + ", Result: ";

                    if (USBDeviceControl.SetRegisterBits(value, register))
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

                    string message = "#" + ((short)register) + " Reset, Value: 0x" + GetHexString(value) + ", Result: ";

                    if (USBDeviceControl.ResetRegisterBits(value, register))
                    {
                        return message + "Success.";
                    }
                    else
                    {
                        return message + "Failure.";
                    }
                }
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


    }
}
