using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DlibDotNet;
using DlibDotNet.Extensions;
using Dlib = DlibDotNet.Dlib;
namespace zoooom.net_core
{

    public partial class Form1 : Form
    {
        private const string path = @"D:\MyZoomScreenShot.png";
        //private string path = @"C:\MyZoomScreenShot.png";
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        public Form1()
        {
            InitializeComponent();
        }

        public void FindFaces()
        {
            using (var fd = Dlib.GetFrontalFaceDetector())
            {
                var img = Dlib.LoadImage<RgbPixel>(path);

                // find all faces in the image
                var faces = fd.Operator(img);
                foreach (var face in faces)
                {
                    // draw a rectangle for each face
                    Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                }
                Dlib.SaveJpeg(img, @"D:\output.png");
            }
        }
        public void PrintScreen(string path)
        {

            try
            {
                var process = Process.GetProcessesByName("zoom", Environment.MachineName);
                if (process.Length == 0)
                {
                    MessageBox.Show("Zoom не был обнаружен");
                    return;
                }
                int[] arr = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < process.Length; i++)
                {
                    arr[i] = process[i].WorkingSet;
                }
                //variable = arr[0] > arr[1] ? 0 : 1;
                int maxVal = arr.Max();
                int variable = Array.FindIndex(arr, x => x == maxVal);
                var hwnd = process[variable].MainWindowHandle;
                GetWindowRect(hwnd, out var rect);
                SetForegroundWindow(process[variable].MainWindowHandle);
                SendKeys.Send("%{PRTSC}");
                Bitmap returnImage = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
                SendKeys.Send("%{PRTSC}");
                IDataObject returPic = Clipboard.GetDataObject();
                SetForegroundWindow(process[variable].MainWindowHandle);
                SendKeys.Send("%{PRTSC}");
                returnImage = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
                SendKeys.Send("%{PRTSC}");
                if (returPic.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    returnImage = (Bitmap)returPic.GetData(DataFormats.Bitmap);

                    string filename = saveFileDialog1.FileName;
                    //MessageBox.Show("Файл сохранен");
                    returnImage.Save(path, ImageFormat.Png);
                }
                else
                {//MessageBox.Show("Попробуйте еще раз)");
                    string filename = saveFileDialog1.FileName;
                    //MessageBox.Show("Файл сохранен");
                    returnImage.Save(path, ImageFormat.Png);
                }
            }
            catch
            {
                MessageBox.Show("Что то пошло не так");
            }




        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            PrintScreen(path);
            FindFaces();
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                long size = file.Length;
                if (size < 3000)
                {
                    PrintScreen(path);
                    FindFaces();
                }
            }
            catch
            {
                { }
            }
            

            //MessageBox.Show("Файл успешно создан");






        }
    }
}
