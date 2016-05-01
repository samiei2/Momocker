using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInputSimulator;

namespace OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for Keyboard.xaml
    /// </summary>
    public partial class Keyboard : UserControl
    {
        public event EventHandler  KeyboardKeyPress;
        List<KeyboardButton> keyboardNonActionKeys;
        public Keyboard()
        {
            InitializeComponent();
            keyboardNonActionKeys = GetAllKeyboardKeysAccessor();
            SubmitToEveryKeyTrigger();
        }

        private void SubmitToEveryKeyTrigger()
        {
            List<KeyboardButton> keys = new List<KeyboardButton>();
            var keyList = GetLogicalChildCollection<KeyboardButton>(this);
            foreach (var item in keyList)
            {
                item.KeyTriggered += new EventHandler(KeyPressed);
            }
        }

        private void KeyPressed(object sender, EventArgs e)
        {
            if (e is KeyPressEventArgs)
            {
                var eventArgs = (KeyPressEventArgs)e;
                KeyboardKeyPress(sender, eventArgs);
            }
        }

        private List<KeyboardButton> GetAllKeyboardKeysAccessor()
        {
            List<KeyboardButton> keys = new List<KeyboardButton>();
            var keyList = GetLogicalChildCollection<KeyboardButton>(this);
            foreach (var item in keyList)
            {
                if(!item.ControlButton)
                    keys.Add(item);
            }
            return keys;
        }

        internal void EnableKeys(List<char> keys)
        {
            foreach (var item in keys)
            {
                var foundKeys = keyboardNonActionKeys.Where<KeyboardButton>(key => { return key.ButtonText.ToLower() == item.ToString(); });
                if (foundKeys != null && foundKeys.Count() > 0)
                {
                    var selectedKey = foundKeys.First<KeyboardButton>();
                    selectedKey.IsEnabled = true;
                }
            }
        }

        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        internal void DisableAllKeys()
        {
            foreach (var item in keyboardNonActionKeys)
            {
                item.IsEnabled = false;
            }
        }
    }
}
