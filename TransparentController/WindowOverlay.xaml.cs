using Gma.System.MouseKeyHook;
using MinimizeCapture;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TransparentController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class WindowOverlay : Window
    {
        System.Timers.Timer _timer = new System.Timers.Timer();
        private IKeyboardMouseEvents m_Events;
        private IntPtr _selectedWindow = IntPtr.Zero;

        public WindowOverlay()
        {
            InitializeComponent();
            Subscribe(Hook.GlobalEvents());
            _timer.AutoReset = true;
            _timer.Interval = 1000;
            _timer.Elapsed += TimedScreenShot;
            _timer.Start();
        }

        private void TimedScreenShot(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_selectedWindow != IntPtr.Zero)
            {
                Dispatcher.Invoke((Action)(() => { image.Source = null; }));
                var bitmap = WindowSnap.GetWindowSnap(_selectedWindow, true).Image;
                Dispatcher.Invoke((Action)(() => { image.Source = BitmapToImageSource(bitmap); }));
            }
        }

        private void SwitchPic(BitmapImage img)
        {
            img.Freeze();
            if (!img.Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)(() => {
                    image.Source = img;
                }),img);
            }
            else
            {
                image.Source = img;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //var hwnd = new WindowInteropHelper(this).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
        }

        

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Win32.GetWindowHandleFromCursor();
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.MouseDown += OnMouseDown;
            m_Events.MouseMove += OnMouseMove;
            m_Events.MouseDragFinished += OnMouseDragFinished;
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;
            

            m_Events.Dispose();
            m_Events = null;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _selectedWindow = Win32.GetForegroundWindow();
            //image.Source = null;
            //var window = Win32.GetForegroundWindow();
            //if (window != IntPtr.Zero) 
            //{
            //    ScreenCapture.CaptureWindow(window);
            //    var bitmap = WindowSnap.GetWindowSnap(window, true).Image;
                //var bitmap = ScreenCapture.CaptureDesktop();
                //image.Source = BitmapToImageSource(bitmap);
                //image.InvalidateVisual();
                //System.Drawing.Point topLeft = Win32.GetControlPosition(window);
                //System.Drawing.Size size = Win32.GetControlSize(window);
                //this.Left = topLeft.X;
                //this.Top = topLeft.Y;
                //this.Height = size.Height;
                //this.Width = size.Width;
                //this.Show();
            //}
            //test();
            //var window = GetWindowHandle();
            //if (window != IntPtr.Zero)
            //{
            //    var bitmap = WindowSnap.GetWindowSnap(window, true).Image;
            //    //var bitmap = ScreenCapture.CaptureDesktop();
            //    image.Source = BitmapToImageSource(bitmap);
            //}
        }

        private void OnMouseDragFinished(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var window = Win32.GetActiveWindow();
            if (window != IntPtr.Zero)
            {
                System.Drawing.Point topLeft = Win32.GetControlPosition(window);
                System.Drawing.Size size = Win32.GetControlSize(window);
                this.Left = topLeft.X;
                this.Top = topLeft.Y;
                this.Height = size.Height;
                this.Width = size.Width;
                this.Show();
            }
        }
        
        private void test()
        {
            System.Drawing.Size sz = Screen.PrimaryScreen.Bounds.Size;
            IntPtr hDesk = Win32.GetDesktopWindow();
            IntPtr hSrce = Win32.GetWindowDC(hDesk);
            IntPtr hDest = Win32.CreateCompatibleDC(hSrce);
            IntPtr hBmp = Win32.CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
            IntPtr hOldBmp = Win32.SelectObject(hDest, hBmp);
            bool b = Win32.BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Bitmap.FromHbitmap(hBmp);
            Win32.SelectObject(hDest, hOldBmp);
            Win32.DeleteObject(hBmp);
            Win32.DeleteDC(hDest);
            Win32.ReleaseDC(hDesk, hSrce);
            //bmp.Save(@"c:\temp\test.png");
            image.Source = null;
            image.Source = BitmapToImageSource(bmp);
            bmp.Dispose();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //textBox.Text = e.X + "," + e.Y;
            //test();
        }
    }
}
