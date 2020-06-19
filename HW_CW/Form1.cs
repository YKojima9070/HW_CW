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
    //public class Form1 : Form
    {

        //private NAIT_Program nait_program;
        //private List<string> str;

 
        int WIDTH = 640;
        int HEIGHT = 480;
        Mat frame;
        VideoCapture capture;
        Bitmap bmp;
        Graphics graphics;


        public Form1()
        {
            InitializeComponent();
  
        }

        /*
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           graphics.DrawImage(bmp, 0, 0, frame.Cols, frame.Rows);     
        }
        */

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)

        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            while (!backgroundWorker1.CancellationPending)
            {
                capture.Grab();

                NativeMethods.videoio_VideoCapture_operatorRightShift_Mat(capture.CvPtr, frame.CvPtr);

                //backgroundWorker1_ProgressChangedを呼び出す、進行状況を知る必要がない場合はいらない
                //今回はフォーマットに従い入れてある。。なくても動く
                //bw.ReportProgress(0);

                graphics.DrawImage(bmp, 0, 0, frame.Cols, frame.Rows);
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
            NAIT_Program nait_program = new NAIT_Program();
            List<string> image_path = nait_program.imagePaths();

            nait_program.model_read(image_path);

            Console.WriteLine(image_path);




        }

        private void buttonTarDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "フォルダを指定してください。";

            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                textBoxTarDir.Clear();
                textBoxTarDir.AppendText(fbd.SelectedPath);

            }
        }

        private void buttonRdMod_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "学習モデル(*.net)|*.net|すべてのファイル(*.*)|*.*";
            ofd.Title = "開くファイルを選択して下さい。";

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                textBoxRdMd.Clear();
                textBoxRdMd.AppendText(ofd.FileName);
            }
        }

        private void buttonNetCam_Click(object sender, EventArgs e)
        {

            string url = textBoxNetCamAddr.Text;
            vlcControl1.Play(new Uri(@url));

        }

        private void vlcControl1_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new System.IO.DirectoryInfo(@"./Debug");
        }
    }
}
