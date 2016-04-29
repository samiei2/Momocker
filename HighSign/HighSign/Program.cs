﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using HighSign.Common.Plugins;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HighSign
{
	static class Program
	{

        #region Temp/To be Removed
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>  
        [STAThread]
		static void Main()
		{
            #region TO be Removed
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowPos(hWnd,
                new IntPtr(HWND_TOPMOST),
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE);
            #endregion

            bool createdNew = true;
			using (Mutex mutex = new Mutex(true, "HighSign", out createdNew))
			{
				if (createdNew)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);

					Input.MouseCapture.Instance.Load();
					Gestures.GestureManager.Instance.Load();
					UI.FormManager.Instance.Load();
					//Drawing.Compatibility.Surface.Instance.Load();
					Applications.ApplicationManager.Instance.Load();
					Plugins.PluginManager.Instance.Load();
					UI.TrayManager.Instance.Load();
					Input.MouseCapture.Instance.EnableMouseCapture();

					//UI.Forms.ActionConfiguration dialog = new HighSign.UI.Forms.ActionConfiguration();
					//dialog.Show();
					
					try
					{
						Process process = Process.GetCurrentProcess();
						process.MinWorkingSet = new IntPtr(300000);
						process.MaxWorkingSet = new IntPtr(6000000) ;
						process.Dispose();
					}
					catch { }					
					
					Application.Run();
				}
			}
		}


	}
}