using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;


namespace HW_CW
{
    public partial class Form1 : Form
    {
        int WIDTH = 640;
        int HEIGHT = 480;
        Mat frame;
        VideoCapture capture;
        Bitmap bmp;
        Graphics graphics;

        public Form1()
        {
            InitializeComponent();
            capture = new VideoCapture(0);

            if (!capture.IsOpened())
            {
                MessageBox.Show("camera was not found!");
                this.Close();

            }

            capture.FrameWidth = WIDTH;
            capture.FrameHeight = HEIGHT;


            frame = new Mat(HEIGHT, WIDTH, MatType.CV_8UC3);

            bmp = new Bitmap(frame.Cols, frame.Rows, (int)frame.Step(), System.Drawing.Imaging.PixelFormat.Format24bppRgb, frame.Data);

            pictureBox1.Width = frame.Cols;
            pictureBox1.Height = frame.Rows;

            graphics = pictureBox1.CreateGraphics();

            backgroundWorker1.RunWorkerAsync();

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)

        {
            graphics.DrawImage(bmp, 0, 0, frame.Cols, frame.Rows);     
        
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)

        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            while (!backgroundWorker1.CancellationPending)
            {

                capture.Grab();

                NativeMethods.videoio_VideoCapture_operatorRightShift_Mat(capture.CvPtr, frame.CvPtr);

                bw.ReportProgress(0);

            }
        
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            backgroundWorker1.CancelAsync();
            while (backgroundWorker1.IsBusy)
                Application.DoEvents();

        }
    }
}
