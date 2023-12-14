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
    class ImageAcqRegisterProperty
    {
        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Auto Exposure Enable")]
        [Description("Auto Exposure Enable")]
        [ReadOnly(false)]
        [Browsable(true)]       
        public bool AutoExpEnable { get; set; }

        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Exposure Time")]
        [Description("Exposure Time")]
        [ReadOnly(false)]
        [Browsable(true)]
        public UInt32 ExpTime { get; set; }

        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Exposure Delay")]
        [Description("Exposure Delay")]
        [ReadOnly(false)]
        [Browsable(true)]
        public UInt32 ExpDelay { get; set; }

        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Auto Exposure Sweep Step Size")]
        [Description("Control the Mid-Gray auto exposure sweep step size")]
        [ReadOnly(false)]
        [Browsable(true)]
        public UInt32 AutoExSweepStep { get; set; }

        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Auto Exposure Interval")]
        [Description("Auto Exposure Interval in 1us step")]
        [ReadOnly(false)]
        [Browsable(true)]
        public UInt32 AutoExInterval { get; set; }

        [CustomSortedCategory("Exposure Control", 1, 5)]
        [DisplayName("Mid-Gray Auto Exposure Skew")]
        [Description("Mid-Gray Auto Exposure Skew")]
        [ReadOnly(false)]
        [Browsable(true)]
        public UInt32 MidGraySkew { get; set; }

        [CustomSortedCategory("Light Source Control", 1, 5)]
        [DisplayName("Single Line Laser Dimming")]
        [Description("Single Line Laser Dimming Value")]
        [ReadOnly(false)]
        [Browsable(true)]
        [UInt32Validation(Min = 0, Max = 100)]
        public UInt32 SLLDim { get; set; }


        [CustomSortedCategory("Light Source Control", 1, 5)]
        [DisplayName("Multi Line Laser Dimming")]
        [Description("Multi Line Laser Dimming Value")]
        [ReadOnly(false)]
        [Browsable(true)]
        [UInt32Validation(Min = 0, Max = 100)]
        public UInt32 MLLDim { get; set; }


        [CustomSortedCategory("Light Source Control", 1, 5)]
        [DisplayName("LED Ring Dimming")]
        [Description("LED Ring Dimming Value")]
        [ReadOnly(false)]
        [Browsable(true)]
        [UInt32Validation(Min = 0, Max = 100)]
        public UInt32 LedDim { get; set; }

        public ImageAcqRegisterProperty()
        {
            AutoExpEnable = false;
            ExpTime = 200;
            ExpDelay = 0;
            AutoExSweepStep = 5;
            AutoExInterval = 300;
            MidGraySkew = 500;
            SLLDim = 50;
            MLLDim = 50;
            LedDim = 50;
        }

        public static void PropertyValueChanged(DynamicTypeDescriptor dt, string propertyName)
        {
            if (propertyName == null) return;

            object newValue = ((DynamicProperty)dt.Properties[propertyName]).Value;

        }

        public void WriteToRegister(ImageAcqRegisterProperty registerProperty)
        {
            // MainRegisterProperty registerProperty = this.Clone();

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(ImageAcqRegisterProperty));
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
