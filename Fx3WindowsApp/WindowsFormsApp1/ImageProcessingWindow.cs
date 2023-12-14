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
    public partial class ImageProcessingWindow : Form
    {
        public static ImageProcessingWindow AppWindow;
        public static PropertyGrid IP_PropertyGrid;
        public static DynamicTypeDescriptor CurrentDynamicTypeDescriptor;
        public ImageProcessingWindow()
        {
            InitializeComponent();
            AppWindow = this;

            IP_PropertyGrid = propertyGridIP;
            InitializePropertyGrid();
        }

        public void InitializePropertyGrid()
        {
            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(typeof(ImageProcessorRegisterProperty));
            ImageProcessorRegisterProperty ipRegister = new ImageProcessorRegisterProperty();
            dt = dt.FromComponent(ipRegister);
            dt.PropertyChanged += PropertyChangedEventHandler;
            IP_PropertyGrid.SelectedObject = dt;
            CurrentDynamicTypeDescriptor = dt;
        }

        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {

            string propertyName = e.PropertyName;
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)sender;
            ImageProcessorRegisterProperty.PropertyValueChanged(dt, propertyName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DynamicTypeDescriptor dt = (DynamicTypeDescriptor)IP_PropertyGrid.SelectedObject;
            ImageProcessorRegisterProperty ipRegister = new ImageProcessorRegisterProperty();
            ipRegister = (ImageProcessorRegisterProperty)dt.Component;
            ipRegister.WriteToRegister(ipRegister);
            this.Close();
        }
    }
}
