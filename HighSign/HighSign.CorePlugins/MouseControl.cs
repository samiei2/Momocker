﻿using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace KinectV2MouseControl
{
    class MouseControl
    {
        public static readonly int WHEEL_DELTA = 120;
        public static void MouseLeftDown()
        {
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
        }
        public static void MouseLeftUp()
        {
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void DoMouseClick()
        {
            mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void DoDoubleMouseClick()
        {
            mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp , 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            mouse_event(MouseEventFlag.LeftDown | MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void MouseRightDown()
        {
            mouse_event(MouseEventFlag.RightDown, 0, 0, 0, UIntPtr.Zero);
        }
        public static void MouseRightUp()
        {
            mouse_event(MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void DoMouseRightClick()
        {
            mouse_event(MouseEventFlag.RightDown | MouseEventFlag.RightUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void Scroll(int times)
        {
            mouse_event(MouseEventFlag.Wheel, 0, 0, (uint)(times * WHEEL_DELTA), UIntPtr.Zero);
        }

        public static void ScrollClick()
        {
            mouse_event(MouseEventFlag.MiddleDown | MouseEventFlag.MiddleUp, 0, 0, 0, UIntPtr.Zero);
        }

        public static void MoveTo(int x,int y)
        {
            mouse_event(MouseEventFlag.Absolute | MouseEventFlag.Move, x, y, 0, UIntPtr.Zero);
        }

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        [Flags]
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }
    }
}
