using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImagerDebugSoftware.PropertiesTools
{
    class CustomSortedCategoryAttribute : CategoryAttribute
    {
        public const char NonPrintableChar = '\t';

        public CustomSortedCategoryAttribute(string category,
                                                ushort categoryPos,
                                                ushort totalCategories)
            : base(category.PadLeft(category.Length + (totalCategories - categoryPos),
                        CustomSortedCategoryAttribute.NonPrintableChar))
        {
        }
    }
}
