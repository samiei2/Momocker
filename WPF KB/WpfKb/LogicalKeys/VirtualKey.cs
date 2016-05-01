using System;
using System.Linq;
using WindowsInput;

namespace WpfKb.LogicalKeys
{
    public class VirtualKey : LogicalKeyBase
    {
        private VirtualKeyCode _keyCode;
        private WindowsInputSimulator.InputSimulator sim = new WindowsInputSimulator.InputSimulator();

        public virtual VirtualKeyCode KeyCode
        {
            get { return _keyCode; }
            set
            {
                if (value != _keyCode)
                {
                    _keyCode = value;
                    OnPropertyChanged("KeyCode");
                }
            }
        }

        public VirtualKey(VirtualKeyCode keyCode, string displayName)
        {
            DisplayName = displayName;
            KeyCode = keyCode;
        }

        public VirtualKey(VirtualKeyCode keyCode)
        {
            KeyCode = keyCode;
        }

        public VirtualKey()
        {
        }

        public override void Press()
        {
            InputSimulator.SimulateKeyPress(_keyCode);
            //var _myKeyCode = ToMySimulatorKeyCode(_keyCode);
            //if(_myKeyCode!= WindowsInputSimulator.Native.VirtualKeyCode.EMPTY)
            //    sim.Keyboard.KeyPress(_myKeyCode);
            base.Press();
        }

        public static WindowsInputSimulator.Native.VirtualKeyCode ToMySimulatorKeyCode(WindowsInput.VirtualKeyCode _keyCode)
        {
            var values2 = Enum.GetValues(typeof(WindowsInputSimulator.Native.VirtualKeyCode)).Cast<WindowsInputSimulator.Native.VirtualKeyCode>();
            foreach (var item in values2)
            {
                if (item.ToString().ToLower() == _keyCode.ToString().ToLower())
                   return item;
            }
            return WindowsInputSimulator.Native.VirtualKeyCode.EMPTY;
        }
    }
}