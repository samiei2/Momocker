using Gma.System.MouseKeyHook;
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

    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out System.Drawing.Point lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private IKeyboardMouseEvents m_Events;

        public MainWindow()
        {
            InitializeComponent();
            Subscribe(Hook.GlobalEvents());
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        private IntPtr GetWindowHandle()
        {
            System.Drawing.Point p;
            if (GetCursorPos(out p))
            {
                IntPtr hWnd = WindowFromPoint(p);
                if (hWnd != IntPtr.Zero)
                {
                    //SetWindowText(hWnd, "Found You: " + p.X + "," + p.Y);
                }
                return hWnd;
            }
            return IntPtr.Zero;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GetWindowHandle();
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.MouseDown += OnMouseDown;
            m_Events.MouseMove += OnMouseMove;
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;
            

            m_Events.Dispose();
            m_Events = null;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var window = GetWindowHandle();
            if (window != IntPtr.Zero)
            {
                var bitmap = ScreenCapture.CaptureWindow(window);
                image.Source = BitmapToImageSource(bitmap);
            }
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
            textBox.Text = e.X + "," + e.Y;
        }
    }
}
