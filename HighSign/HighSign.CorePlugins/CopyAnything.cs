using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HighSign.Common.Plugins;
using ManagedWinapi.Windows;
using WindowsInput;
using KinectV2MouseControl;
using System.Runtime.InteropServices;

namespace HighSign.CorePlugins
{
	public class CopyAnything : IPlugin
	{
		#region Private Variables

		IHostControl _HostControl = null;
        InputSimulator inputSim = new InputSimulator();

		#endregion

		#region Public Properties

		public string Name
		{
			get { return "Copy"; }
		}

		public string Description
		{
			get { return "Copy anything that supports Ctrl+C"; }
		}

		public Form GUI
		{
			get { return null; }
		}

		public string Category
		{
			get { return "Windows"; }
		}

		public bool IsAction
		{
			get { return true; }
		}

		#endregion

		#region Public Methods

		public void Initialize()
		{

		}

		public void ShowGUI(bool IsNew)
		{
			// Nothing to do here
		}

		public bool Gestured(Common.Plugins.PointInfo ActionPoint)
		{
			// Don't attempt to minimize tool windows (including Windows Program Manager)
			if ((ActionPoint.Window.ExtendedStyle & WindowExStyleFlags.TOOLWINDOW) == WindowExStyleFlags.TOOLWINDOW)
				return false;

            // Minimize window
            var windowHandle = ActionPoint.WindowHandle;
            inputSim.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);
			//ActionPoint.Window.WindowState = System.Windows.Forms.FormWindowState.Minimized;

			return true;
		}

		public void Deserialize(string SerializedData)
		{
			// Nothing to do here
		}

		public string Serialize()
		{
			// Nothing to serialize
			return "";
		}

		#endregion

		#region Host Control

		public IHostControl HostControl
		{
			get { return _HostControl; }
			set { _HostControl = value; }
		}

        #endregion

        #region WIN32
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
        // second overload of SendMessage
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint Msg, out int wParam, out int lParam);

        const uint WM_GETTEXT = 0x0D;
        const uint WM_GETTEXTLENGTH = 0x0E;
        const uint EM_GETSEL = 0xB0;

        private string GetSelectedText()
        {

            IntPtr hWnd = GetForegroundWindow();

            uint processId;

            uint activeThreadId = GetWindowThreadProcessId(hWnd, out processId);
            uint currentThreadId = GetCurrentThreadId();
            AttachThreadInput(activeThreadId, currentThreadId, true);
            IntPtr focusedHandle = GetFocus();
            AttachThreadInput(activeThreadId, currentThreadId, true);
            StringBuilder sb1 = new StringBuilder();
            int len = SendMessage(focusedHandle, WM_GETTEXTLENGTH, 0, sb1);

            StringBuilder sb = new StringBuilder(len);
            int numChars = SendMessage(focusedHandle, WM_GETTEXT, len + 1, sb);
            int start, next;
            SendMessage(focusedHandle, EM_GETSEL, out start, out next);
            string selectedText = "";
            if (sb.ToString() != "")
                selectedText = sb.ToString().Substring(start, next - start);

            return selectedText;
        }

        #endregion
    }
}
