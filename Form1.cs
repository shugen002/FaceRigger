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

namespace faceRigger
{
    public partial class Form1 : Form
    {
        private Process process = null;
        private int status = 0;
        private Color noneColor = Color.Gray;
        private Color errorColor = Color.Red;
        private Color notTrackingColor = Color.Yellow;
        private Color trackingColor = Color.Green;
        private bool locked = false;
        private bool moving = false;
        private bool inside = false;
        private Point StartPoint;
        private Point OffsetPoint = new Point(0, 0);
        public static int GWL_EXSTYLE = -20;
        public static int WS_EX_TRANSPARENT = 0x20;
        public static int WS_EX_LAYERED = 0x80000;

        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public Form1()
        {
            InitializeComponent();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= WS_EX_LAYERED;
                return cp;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (process != null && !process.HasExited)
            {
                if (!process.Responding)
                {
                    status = -1;
                }
                else
                {
                    try
                    {
                        int tracking = memory.ReadMemoryInt32(process.Id, 0x01C74A64);
                        if (tracking == 0)
                        {
                            status = 2;
                        }
                        else if (tracking == 1)
                        {
                            status = 1;
                        }
                        else
                        {
                            process = null;
                            status = 0;
                        }
                    }
                    catch (Exception)
                    {
                        process = null;
                        status = 0;
                        throw;
                    }
                }
            }
            else
            {
                process = checkRunning();
                if (process == null)
                {
                    status = 0;
                }
            }
            switch (status)
            {
                case -1:
                    this.BackColor = errorColor;
                    break;
                case 0:
                    this.BackColor = noneColor;
                    break;
                case 1:
                    this.BackColor = trackingColor;
                    break;
                case 2:
                    this.BackColor = notTrackingColor;
                    break;
                default:
                    this.BackColor = noneColor;
                    break;
            }
        }


        public Process checkRunning()
        {
            Process[] processes = Process.GetProcessesByName("FaceRig");
            if (processes.Length < 0)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    try
                    {
                        if (processes[i].MainWindowTitle == "FaceRig" || processes[i].MainWindowTitle == "FaceRig Secondary Render Window")
                        {
                            return processes[i];
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                return null;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!locked && inside)
                {
                    moving = true;
                    StartPoint = e.Location;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!locked && inside && moving)
            {
                OffsetPoint.X = e.X - StartPoint.X;
                OffsetPoint.Y = e.Y - StartPoint.Y;
                Location = PointToScreen(OffsetPoint);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!locked)
            {
                moving = false;
            }
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            inside = true;
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            inside = false;
        }

        private void From1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!locked && inside)
            {

                if (this.Width > 1 && e.Delta < 0)
                {
                    this.Width = Math.Max(1, this.Width + e.Delta / 120);
                    this.Height = Math.Max(1, this.Height + e.Delta / 120);
                }
                if (this.Width < 256 && e.Delta > 0)
                {
                    this.Width = Math.Min(256, this.Width + e.Delta / 120);
                    this.Height = Math.Min(256, this.Height + e.Delta / 120);
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                locked = true;
                inside = false;
                moving = false;
                IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
                int style = GetWindowLong(mainWindowHandle, GWL_EXSTYLE);
                SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_TRANSPARENT);
            }
        }
    }
}
