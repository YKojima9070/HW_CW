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

        private NAIT_Program naitProgram;

        int WIDTH = 640;
        int HEIGHT = 480;
        Mat frame;
        VideoCapture capture;
        Bitmap bmp;
        Graphics graphics;

        public Form1()
        {
            naitProgram = new NAIT_Program();

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

                //backgroundWorker1_ProgressChangedを呼び出す、進行状況を知る必要がない場合はいらない
                //今回はフォーマットに従い入れてある。。なくても動く
                bw.ReportProgress(0);
            }

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            backgroundWorker1.CancelAsync();
            while (backgroundWorker1.IsBusy)
                Application.DoEvents();

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void buttonSTOP_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backgroundWorker1.CancelAsync();
            }
        }

        private void buttonNaitLoad_Click(object sender, EventArgs e)
        {

            naitProgram.classificaition_example();

        }
    }
}
