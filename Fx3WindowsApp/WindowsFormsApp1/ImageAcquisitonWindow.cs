using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyUSB;
using ImagerDebugSoftware.PropertiesTools;
using UsbApi;

namespace ImagerDebugSoftware
{
    public partial class ImageAcquisitonWindow : Form
    {
        public static ImageAcquisitonWindow AppWindow;
        public static PropertyGrid IA_PropertyGrid;
        public static DynamicTypeDescriptor CurrentDynamicTypeDescriptor;
        public ImageAcquisitonWindow()
        {
            InitializeComponent();
            AppWindow = this;

            IA_PropertyGrid = propertyGridIA;
            InitializePropertyGrid();
        }
        public void InitializePropertyGrid()
        {
            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(ImageAcqRegisterProperty));
            ImageAcqRegisterProperty iaRegister = new ImageAcqRegisterProperty();
            dt = dt.FromComponent(iaRegister);
            dt.PropertyChanged += PropertyChangedEventHandler;
            IA_PropertyGrid.SelectedObject = dt;
            CurrentDynamicTypeDescriptor = dt;
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {

            string propertyName = e.PropertyName;
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)sender;
            MainRegisterProperty.PropertyValueChanged(dt, propertyName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented in hardware! Demo Only!");
        }
    }
}
