using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Auto_Weapon_Detection
{
    public partial class Menu : Form
    {
        
        public Menu()
        {
            
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            
            Detector.RunWorkerAsync();           
        }

        
        private void Detector_DoWork(object sender, DoWorkEventArgs e)
        {

            Graphics g;
            Bitmap bmp;

            bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width / 8, Screen.PrimaryScreen.Bounds.Height / 4);
            g = Graphics.FromImage(bmp);

            Image<Gray, Byte>[] guns = new Image<Gray, Byte>[] {
                    new Image<Gray, Byte>(@"./img/Melee.png"),
                    new Image<Gray, Byte>(@"./img/Classic.png"),
                    new Image<Gray, Byte>(@"./img/Shorty.png"),
                    new Image<Gray, Byte>(@"./img/Frenzy.png"),
                    new Image<Gray, Byte>(@"./img/Ghost.png"),
                    new Image<Gray, Byte>(@"./img/Sheriff.png"),
                    new Image<Gray, Byte>(@"./img/Stinger.png"),
                    new Image<Gray, Byte>(@"./img/Spectre.png"),
                    new Image<Gray, Byte>(@"./img/Bucky.png"),
                    new Image<Gray, Byte>(@"./img/Judge.png"),
                    new Image<Gray, Byte>(@"./img/Bulldog.png"),
                    new Image<Gray, Byte>(@"./img/Guardian.png"),
                    new Image<Gray, Byte>(@"./img/Phantom.png"),
                    new Image<Gray, Byte>(@"./img/Vandal.png"),
                    new Image<Gray, Byte>(@"./img/Marshal.png"),
                    new Image<Gray, Byte>(@"./img/Operator.png"),
                    new Image<Gray, Byte>(@"./img/Ares.png"),
                    new Image<Gray, Byte>(@"./img/Odin.png")
                };

            string[] gunNames = new string[]
            {
                 "Melee", "Classic", "Shorty", "Frenzy", "Ghost", "Sheriff", "Stinger", "Spectre", "Bucky", "Judge", "Bulldog", "Guardian", "Phantom", "Vandal", "Marshal", "Operator", "Ares", "Odin"
            };

            List<double> confidences = new List<double>();


            while (true)
            {

                confidences.Clear();
                this.TopMost = true;
                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.Width / 6 * 5, Screen.PrimaryScreen.Bounds.Height / 24 * 15, 0, 0, new Size(bmp.Width, bmp.Height));              

                Image<Gray, Byte> sourceImage = bmp.ToImage<Gray, Byte>();
                sourceImage = sourceImage.ThresholdBinary(new Gray(160), new Gray(255));

                List<double> scores = new List<double>();

                for (int i = 0; i < guns.Length; i++)
                {
                    
                    Image<Gray, float> resultImage = sourceImage.MatchTemplate(guns[i], TemplateMatchingType.CcoeffNormed);
                    float[,,] matches = resultImage.Data;
                    for (int y = 0; y < matches.GetLength(0); y++)
                    {
                        for (int x = 0; x < matches.GetLength(1); x++)
                        {
                            double matchScore = matches[y, x, 0];
                            scores.Add(matchScore);
                        }
                    }

                    confidences.Add(scores.Max());
                    scores.Clear();
                }

                double max = confidences.Max();

                if (max > 0.8)
                    lblLastWeapon.Text = gunNames[confidences.IndexOf(max)];
                
                Thread.Sleep(1);
            }
        }



        //
        // UI Stuff
        //

        [DllImport("user32.dll")]
        public static extern bool AllowSetForegroundWindow(uint dwProcessId);
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        private void mouseDrag(object sender, MouseEventArgs e)
        {
           
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}