using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Gma.System.MouseKeyHook;

namespace TransparentController
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private IKeyboardMouseEvents m_Events;
        System.Timers.Timer _timer;
        List<string> selectedFiles;
        public Window1()
        {
            InitializeComponent();
            Subscribe(Hook.GlobalEvents());
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = 100;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            this.Left = 0;
            //_timer = new System.Timers.Timer(100);
            //_timer.Elapsed += TimeElapsed;
            //_timer.Start();
        }

        //private void TimeElapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    selectedFiles = GetSelectedFiles();
        //}

        private static List<string> GetSelectedFiles()
        {
            IntPtr handle = Win32.GetForegroundWindow();

            List<string> selected = new List<string>();
            var shell = new Shell32.Shell();
            foreach (SHDocVw.InternetExplorer window in shell.Windows())
            {
                if (window.HWND == (int)handle)
                {
                    Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
                    foreach (Shell32.FolderItem item in items)
                    {
                        selected.Add(item.Path);
                    }
                }
            }
            return selected;
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.MouseDown += OnMouseDown;
            m_Events.MouseUp += OnMouseUp;
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            List<string> selectedFiles = null;
            Thread _t = new Thread(() => {
                selectedFiles = GetSelectedFiles();
            });
            _t.SetApartmentState(ApartmentState.STA);
            _t.Start();
            _t.Join();
            this.label1.Content = selectedFiles.Count;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;


            m_Events.Dispose();
            m_Events = null;
        }
    }
}
