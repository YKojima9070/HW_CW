namespace HW_CW
{
    partial class Form1
//    class Form1

    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonSTOP = new System.Windows.Forms.Button();
            this.buttonNaitLoad = new System.Windows.Forms.Button();
            this.buttonTarDir = new System.Windows.Forms.Button();
            this.textBoxTarDir = new System.Windows.Forms.TextBox();
            this.buttonRdMod = new System.Windows.Forms.Button();
            this.textBoxRdMd = new System.Windows.Forms.TextBox();
            this.buttonNetCam = new System.Windows.Forms.Button();
            this.textBoxNetCamAddr = new System.Windows.Forms.TextBox();
            this.vlcControl1 = new Vlc.DotNet.Forms.VlcControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(5, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(640, 480);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(668, 22);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(155, 46);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "USB CAM START";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonSTOP
            // 
            this.buttonSTOP.Location = new System.Drawing.Point(847, 22);
            this.buttonSTOP.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSTOP.Name = "buttonSTOP";
            this.buttonSTOP.Size = new System.Drawing.Size(154, 46);
            this.buttonSTOP.TabIndex = 2;
            this.buttonSTOP.Text = "STOP";
            this.buttonSTOP.UseVisualStyleBackColor = true;
            this.buttonSTOP.Click += new System.EventHandler(this.buttonSTOP_Click);
            // 
            // buttonNaitLoad
            // 
            this.buttonNaitLoad.Location = new System.Drawing.Point(665, 443);
            this.buttonNaitLoad.Name = "buttonNaitLoad";
            this.buttonNaitLoad.Size = new System.Drawing.Size(153, 43);
            this.buttonNaitLoad.TabIndex = 3;
            this.buttonNaitLoad.Text = "NAIT Load";
            this.buttonNaitLoad.UseVisualStyleBackColor = true;
            this.buttonNaitLoad.Click += new System.EventHandler(this.buttonNaitLoad_Click);
            // 
            // buttonTarDir
            // 
            this.buttonTarDir.Location = new System.Drawing.Point(665, 202);
            this.buttonTarDir.Name = "buttonTarDir";
            this.buttonTarDir.Size = new System.Drawing.Size(155, 45);
            this.buttonTarDir.TabIndex = 4;
            this.buttonTarDir.Text = "参照先フォルダ";
            this.buttonTarDir.UseVisualStyleBackColor = true;
            this.buttonTarDir.Click += new System.EventHandler(this.buttonTarDir_Click);
            // 
            // textBoxTarDir
            // 
            this.textBoxTarDir.Location = new System.Drawing.Point(665, 253);
            this.textBoxTarDir.Name = "textBoxTarDir";
            this.textBoxTarDir.Size = new System.Drawing.Size(334, 19);
            this.textBoxTarDir.TabIndex = 5;
            // 
            // buttonRdMod
            // 
            this.buttonRdMod.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonRdMod.Location = new System.Drawing.Point(668, 296);
            this.buttonRdMod.Name = "buttonRdMod";
            this.buttonRdMod.Size = new System.Drawing.Size(150, 45);
            this.buttonRdMod.TabIndex = 6;
            this.buttonRdMod.Text = "モデル読込";
            this.buttonRdMod.UseVisualStyleBackColor = true;
            this.buttonRdMod.Click += new System.EventHandler(this.buttonRdMod_Click);
            // 
            // textBoxRdMd
            // 
            this.textBoxRdMd.Location = new System.Drawing.Point(668, 347);
            this.textBoxRdMd.Name = "textBoxRdMd";
            this.textBoxRdMd.Size = new System.Drawing.Size(333, 19);
            this.textBoxRdMd.TabIndex = 7;
            // 
            // buttonNetCam
            // 
            this.buttonNetCam.Location = new System.Drawing.Point(668, 80);
            this.buttonNetCam.Name = "buttonNetCam";
            this.buttonNetCam.Size = new System.Drawing.Size(154, 43);
            this.buttonNetCam.TabIndex = 8;
            this.buttonNetCam.Text = "NetworkCAM START";
            this.buttonNetCam.UseVisualStyleBackColor = true;
            this.buttonNetCam.Click += new System.EventHandler(this.buttonNetCam_Click);
            // 
            // textBoxNetCamAddr
            // 
            this.textBoxNetCamAddr.Location = new System.Drawing.Point(668, 134);
            this.textBoxNetCamAddr.Name = "textBoxNetCamAddr";
            this.textBoxNetCamAddr.Size = new System.Drawing.Size(330, 19);
            this.textBoxNetCamAddr.TabIndex = 9;
            this.textBoxNetCamAddr.Text = "rtsp://192.168.1.9:554/stream2/sensor1";
            // 
            // vlcControl1
            // 
            this.vlcControl1.BackColor = System.Drawing.Color.Black;
            this.vlcControl1.Location = new System.Drawing.Point(861, 414);
            this.vlcControl1.Name = "vlcControl1";
            this.vlcControl1.Size = new System.Drawing.Size(92, 72);
            this.vlcControl1.Spu = -1;
            this.vlcControl1.TabIndex = 10;
            this.vlcControl1.Text = "vlcControl1";
            this.vlcControl1.VlcLibDirectory = ((System.IO.DirectoryInfo)(resources.GetObject("vlcControl1.VlcLibDirectory")));
            this.vlcControl1.VlcMediaplayerOptions = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 499);
            this.Controls.Add(this.vlcControl1);
            this.Controls.Add(this.textBoxNetCamAddr);
            this.Controls.Add(this.buttonNetCam);
            this.Controls.Add(this.textBoxRdMd);
            this.Controls.Add(this.buttonRdMod);
            this.Controls.Add(this.textBoxTarDir);
            this.Controls.Add(this.buttonTarDir);
            this.Controls.Add(this.buttonNaitLoad);
            this.Controls.Add(this.buttonSTOP);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "ImageViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonSTOP;
        private System.Windows.Forms.Button buttonNaitLoad;
        private System.Windows.Forms.Button buttonTarDir;
        private System.Windows.Forms.TextBox textBoxTarDir;
        private System.Windows.Forms.Button buttonRdMod;
        private System.Windows.Forms.TextBox textBoxRdMd;
        private System.Windows.Forms.Button buttonNetCam;
        private System.Windows.Forms.TextBox textBoxNetCamAddr;
        private Vlc.DotNet.Forms.VlcControl vlcControl1;
    }
}

