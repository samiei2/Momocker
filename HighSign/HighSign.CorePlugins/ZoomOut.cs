using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HighSign.Common.Plugins;
using ManagedWinapi.Windows;
using WindowsInput;
using KinectV2MouseControl;

namespace HighSign.CorePlugins
{
	public class ZoomOut : IPlugin
	{
		#region Private Variables

		IHostControl _HostControl = null;
        InputSimulator inputSim = new InputSimulator();

		#endregion

		#region Public Properties

		public string Name
		{
			get { return "ZoomOut"; }
		}

		public string Description
		{
			get { return "Zoom out for windows that support in"; }
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
            inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL).Mouse.VerticalScroll(-3).Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);
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
	}
}
