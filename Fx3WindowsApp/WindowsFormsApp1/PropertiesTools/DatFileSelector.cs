using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ImagerDebugSoftware.PropertiesTools
{
    class DatFileSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            DynamicTypeDescriptor dynamicTypeDescriptor = context.Instance as DynamicTypeDescriptor;
            MainRegisterProperty registerProperty = dynamicTypeDescriptor.Component as MainRegisterProperty;

            if (edSvc != null)
            {               
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        AddExtension = false,
                        Title = "Please select a file (*.dat)",
                        Filter = "*.dat|*.dat"
                    };
                    if (dialog.ShowDialog().Equals(DialogResult.OK))
                    {
                        return dialog.FileName;
                    }

            }
            return value;
        }
    }
}
