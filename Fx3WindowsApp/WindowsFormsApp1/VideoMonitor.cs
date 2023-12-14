using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImagerDebugSoftware.Tasks;

namespace ImagerDebugSoftware
{
    public partial class VideoMonitor : Form
    {
        private VideoCapture VideoCaptureTask;

        public VideoMonitor()
        {
            InitializeComponent();
            VideoCaptureTask = new VideoCapture(this);
        }

        private void VideoMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            VideoCaptureTask.FinishTask();            
        }

        private void VideoMonitor_Load(object sender, EventArgs e)
        {
            VideoCaptureTask.VideoCaptureStart();
        }

        private void VideoMonitor_SizeChanged(object sender, EventArgs e)
        {            
            VideoCaptureTask.ClearImage();            
        }
    }
}

