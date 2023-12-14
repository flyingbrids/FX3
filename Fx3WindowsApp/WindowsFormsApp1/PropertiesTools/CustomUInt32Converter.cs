using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImagerDebugSoftware.PropertiesTools
{

  public class CustomUInt32Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {

            //, IntValidation(Pattern = @"(?<=19)\d{2}\b")

            object intValidationAttributeObject = context.PropertyDescriptor.Attributes[typeof(UInt32ValidationAttribute)];

            uint result = 0;
            if (value is string)
            {
                if (value != null)
                {

                    //Value was either too large or too small for an Int32.
                    //result = int.Parse((value as string).Replace(",", ""));

                    try
                    {
                        result = uint.Parse((value as string).Replace(",", ""));
                    }
                    catch (OverflowException e)
                    {
                        throw new OverflowException("The value (" + (value as string) + ") is either too large or too small.");
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException("Input string (" + (value as string) + ") is not in a correct format.");
                    }

                }

                if (intValidationAttributeObject != null)
                {

                    UInt32ValidationAttribute intValidationAttribute = (UInt32ValidationAttribute)intValidationAttributeObject;

                    uint min = intValidationAttribute.Min;
                    uint max = intValidationAttribute.Max;

                    if (result < min)
                    {
                        //result = min;
                        throw new OverflowException("The value ( " + (value as string) + " ) is too small. The range of the value should be in [" + min + ", " + max + "]");
                    }
                    else if (result > max)
                    {
                        //result = max;
                        throw new OverflowException("The value ( " + (value as string) + " ) is too large. The range of the value should be in [" + min + ", " + max + "]");
                    }

                }
                return result;
            }
            return 0; // always return something
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is int)
            {
                return ((int)value).ToString("N0");
            }

            return base.ConvertTo(context, culture, value, destinationType); //I left it here but it should never call it
        }
    }
}
