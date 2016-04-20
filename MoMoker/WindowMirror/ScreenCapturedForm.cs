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
using Gma.System.MouseKeyHook;
using MinimizeCapture;
using TransparentController.Captureing;
using WindowsInput;

namespace TransparentController
{
    public partial class ScreenCapturedForm : Form
    {
        System.Timers.Timer _timer = new System.Timers.Timer(50);
        private readonly int FormHeightMargin;
        private readonly int FormWidthMargin; 
        private IKeyboardMouseEvents m_Events;
        private IntPtr _myHandle;
        private IntPtr windowHandle;
        private IntPtr prevWindowPtr;
        
        public ScreenCapturedForm()
        {
            InitializeComponent();
            Subscribe(Hook.GlobalEvents());
            _timer.Elapsed += Elapsed;
            _timer.Start();
            FormHeightMargin = this.Height - this.pictureBox1.Height;
            FormWidthMargin = this.Width - this.pictureBox1.Width;
            _myHandle = this.Handle;
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Image bitmap = null;
            windowHandle = Win32.GetForegroundWindow();

            if (windowHandle == _myHandle)
            {
                //BeginInvoke((MethodInvoker)(() =>
                //{
                //    this.pictureBox1.Image = null;
                //}));
                //return;
                windowHandle = prevWindowPtr;
            }
                
            Win32.RECT windowRect = new Win32.RECT();
            Win32.GetWindowRect(windowHandle, out windowRect);
            if (isWindowOffScreen(windowRect))
            {
                //useOtherPrint();
            }
            else
            {
                bitmap = CaptureUtil.PrintWindow(windowHandle);
                //bitmap = CaptureUtil.CaptureWindowBitBlt(windowHandle);
            }
            if (bitmap == null)
                return;

            prevWindowPtr = windowHandle;

            BeginInvoke((MethodInvoker)(()=>{
                this.pictureBox1.Height = bitmap.Height;
                this.pictureBox1.Width = bitmap.Width;
                this.Height = bitmap.Height + FormHeightMargin;
                this.Width = bitmap.Width + FormWidthMargin;
                if(this.pictureBox1.Image != null)
                    this.pictureBox1.Image.Dispose();
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

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private Point GetAbsolutePosition(Point relativePosition)
        {
            //var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            //var screenHeight = Screen.PrimaryScreen.Bounds.Height;
            Win32.RECT rect = new Win32.RECT();
            Win32.GetWindowRect(prevWindowPtr,out rect);
            //var picBoxWidth = this.pictureBox1.Width;
            //var picBoxHeight = this.pictureBox1.Height;
            //var absXposition = relativePosition.X * screenWidth / picBoxWidth;
            //var absYposition = relativePosition.Y * screenHeight / picBoxHeight;
            return new Point(relativePosition.X + rect.Left, relativePosition.Y + rect.Top);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var relativePosition = pictureBox1.PointToClient(Cursor.Position);
            var oldPosition = Cursor.Position;
            var absPosition = GetAbsolutePosition(relativePosition);
            Win32.RECT rect = new Win32.RECT();
            Win32.GetWindowRect(prevWindowPtr, out rect);
            this.label1.Text = "Window:("+rect.X+","+rect.Y+") // "+ "Rel Pose:("+relativePosition.X+","+relativePosition.Y+")" +
                "ABS Pose:("+ absPosition.X + "," + absPosition.Y+")";
            Cursor.Position = absPosition;
            var simulator = new InputSimulator();
            simulator.Mouse.LeftButtonClick();
            //simulator.Mouse.MoveMouseTo(absPosition.X, absPosition.Y);
            Cursor.Position = oldPosition;
        }
    }
}
