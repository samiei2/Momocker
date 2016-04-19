using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using MinimizeCapture;
using TransparentController.Captureing;

namespace TransparentController
{
    public partial class ScreenCapturedForm : Form
    {
        System.Timers.Timer _timer = new System.Timers.Timer(50);
        private readonly int FormHeightMargin;
        private readonly int FormWidthMargin;
        
        public ScreenCapturedForm()
        {
            InitializeComponent();
            _timer.Elapsed += Elapsed;
            _timer.Start();
            FormHeightMargin = this.Height - this.pictureBox1.Height;
            FormWidthMargin = this.Width - this.pictureBox1.Width;
        }

        private void Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Image bitmap = null;
            var windowHandle = Win32.GetForegroundWindow();
            Win32.RECT windowRect = new Win32.RECT();
            Win32.GetWindowRect(windowHandle, out windowRect);
            if (isWindowOffScreen(windowRect))
            {
                //useOtherPrint();
            }
            else
            {
                bitmap = CaptureUtil.CaptureWindowBitBlt(windowHandle);
            }
            if (bitmap == null)
                return;
            
            BeginInvoke((MethodInvoker)(()=>{
                this.pictureBox1.Height = bitmap.Height;
                this.pictureBox1.Width = bitmap.Width;
                this.Height = bitmap.Height + FormHeightMargin;
                this.Width = bitmap.Width + FormWidthMargin;
                this.pictureBox1.Image = bitmap;
                this.Invalidate();
            }),bitmap);
        }

        private bool isWindowOffScreen(Win32.RECT windowRect)
        {
            var windowHeight = windowRect.Bottom - windowRect.Top;
            var windowWidth = windowRect.Right - windowRect.Left;
            //if(windowRect.Top < Screen.PrimaryScreen.Bounds)
            return false;
        }

        private bool isWindowOffScreen(IntPtr foregroundWindowHandle)
        {
            Win32.RECT windowRect = new Win32.RECT();
            Win32.GetWindowRect(foregroundWindowHandle, out windowRect);
            return isWindowOffScreen(windowRect);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {

        }
    }
}
