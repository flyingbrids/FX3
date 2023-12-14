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
    class PeripheralProperty
    {
        [CustomSortedCategory("Peripheral Settings", 1, 5)]
        [DisplayName("Fan Speed Control")]
        [Description("PWM duty cycle in % of Fan")]
        [ReadOnly(false)]
        [Browsable(true)]
        [UInt32Validation(Min = 0, Max = 100)]
        public UInt32 FanPWM { get; set; }

        [CustomSortedCategory("Peripheral Settings", 1, 5)]
        [DisplayName("User LED Ring Dim")]
        [Description("User LED Ring Dimming PWM duty cycle %")]
        [ReadOnly(false)]
        [Browsable(true)]
        [UInt32Validation(Min = 0, Max = 100)]
        public UInt32 LEDRingDim { get; set; }

        public PeripheralProperty()
        {
            FanPWM = 50;
            LEDRingDim = 50;
        }

        public static void PropertyValueChanged(DynamicTypeDescriptor dt, string propertyName)
        {
            if (propertyName == null) return;

            object newValue = ((DynamicProperty)dt.Properties[propertyName]).Value;

        }

        public void WriteToRegister(PeripheralProperty registerProperty)
        {
            // MainRegisterProperty registerProperty = this.Clone();

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(PeripheralProperty));
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
