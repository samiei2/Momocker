using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnScreenKeyboard
{
    class KeyPressEventArgs : EventArgs
    {
        public DateTime SignalTime { get; set; }

        public bool ControlButton { get; set; }

        public string ButtonText { get; set; }
    }
}
