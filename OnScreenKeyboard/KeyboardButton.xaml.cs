using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for KeyboardButton.xaml
    /// </summary>
    public partial class KeyboardButton : UserControl
    {
        public static readonly DependencyProperty SomeTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(KeyboardButton), new PropertyMetadata(""));
        public static readonly DependencyProperty ControlButtonProperty =
        DependencyProperty.Register("ControlButton", typeof(bool), typeof(KeyboardButton), new PropertyMetadata(false));
        
        System.Timers.Timer _hoverTimer;
        public event EventHandler KeyTriggered;

        public KeyboardButton()
        {
            InitializeComponent();
            _hoverTimer = new System.Timers.Timer(800);
            _hoverTimer.Elapsed += TimerKeyTriggered;
            _hoverTimer.AutoReset = false;
        }

        private void TimerKeyTriggered(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                KeyPressEventArgs keyPress = new KeyPressEventArgs();
                keyPress.SignalTime = e.SignalTime;
                bool controlButton = false;
                string buttonText = "";
                Dispatcher.Invoke((Action)(() => {
                    controlButton = this.ControlButton;
                    buttonText = this.ButtonText;
                }));
                keyPress.ControlButton = controlButton;
                keyPress.ButtonText = buttonText;
                KeyTriggered(sender, keyPress);
            }
            catch(Exception ex){

            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            _hoverTimer.Start();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _hoverTimer.Stop();
        }

        public static void SetControlButtonSource(UIElement element, Boolean value)
        {
            element.SetValue(ControlButtonProperty, value);
        }
        public static Boolean GetControlButtonSource(UIElement element)
        {
            return (Boolean)element.GetValue(ControlButtonProperty);
        }

        public bool ControlButton
        {
            get { return (bool)GetValue(ControlButtonProperty); }
            set { SetValue(ControlButtonProperty, value); }
        }

        public string ButtonText
        {
            get { return (string)GetValue(SomeTextProperty); }
            set { SetValue(SomeTextProperty, value); }
        }
    }

    public class Extensions
    {
        public static readonly DependencyProperty ControlButtonProperty =
        DependencyProperty.Register("ControlButtonProp", typeof(bool), typeof(KeyboardButton), new PropertyMetadata(false));

        public static void SetControlButtonSource(UIElement element, Boolean value)
        {
            element.SetValue(ControlButtonProperty, value);
        }
        public static Boolean GetControlButtonSource(UIElement element)
        {
            return (Boolean)element.GetValue(ControlButtonProperty);
        }
    }
}
