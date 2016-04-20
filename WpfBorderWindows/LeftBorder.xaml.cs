using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WpfBorderWindows
{
    /// <summary>
    /// Interaction logic for LeftBorder.xaml
    /// </summary>
    public partial class LeftBorderWindow : Window
    {
        private IKeyboardMouseEvents m_Events;

        public double Right { get; private set; }

        public LeftBorderWindow()
        {
            InitializeComponent();
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Width = 100;
            this.Top = 0;
            this.Left = 0;
            this.Right = this.Left + this.Width;
            this.Visibility = Visibility.Hidden;
            Subscribe(Hook.GlobalEvents());
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X < this.Right)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = Visibility.Hidden;
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
