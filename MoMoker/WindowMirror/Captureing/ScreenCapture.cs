using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace TransparentController.Captureing
{
    public class CaptureUtil
    {
        public static Image CaptureScreen()
        {
            return CaptureWindowBitBlt(Win32.GetDesktopWindow());
        }

        public static Image CaptureWindowBitBlt(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = Win32.GetWindowDC(handle);
            // get the size
            Win32.RECT windowRect  = new Win32.RECT();
            Win32.GetWindowRect(handle, out windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            // create a device context we can copy to
            IntPtr hdcDest = Win32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = Win32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = Win32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            Win32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Win32.SRCCOPY);
            // restore selection
            Win32.SelectObject(hdcDest, hOld);
            // clean up 
            Win32.DeleteDC(hdcDest);
            Win32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            Win32.DeleteObject(hBitmap);
            return img;
        }
        
        public static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindowBitBlt(handle);
            img.Save(filename, format);
        }
        
        public static void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        public static Bitmap CaptureDesktop()
        {
            return CaptureWindowDotNet(Win32.GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindowDotNet(Win32.GetForegroundWindow());
        }

        public static Bitmap CaptureWindowDotNet(IntPtr handle)
        {
            var rect = new Win32.RECT();
            Win32.GetWindowRect(handle, out rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }

        public static Bitmap PrintWindow(IntPtr hwnd)
        {
            Win32.RECT rc;
            Win32.GetWindowRect(hwnd, out rc);

            if (rc.Height == 0 || rc.Width == 0)
                return null;
            Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            Win32.PrintWindow(hwnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            return bmp;
        }

        //public static Bitmap PrintWindow2(IntPtr hwnd)
        //{
        //    Win32.RECT rc;
        //    Win32.GetWindowRect(hwnd, out rc);

        //    Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
        //    Graphics gfxBmp = Graphics.FromImage(bmp);
        //    IntPtr hdcBitmap = gfxBmp.GetHdc();
        //    bool succeeded = Win32.PrintWindow(hwnd, hdcBitmap, 0);
        //    gfxBmp.ReleaseHdc(hdcBitmap);
        //    if (!succeeded)
        //    {
        //        gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
        //    }
        //    IntPtr hRgn = Win32.CreateRectRgn(0, 0, 0, 0);
        //    Win32.GetWindowRgn(hwnd, hRgn);
        //    Region region = Region.FromHrgn(hRgn);
        //    if (!region.IsEmpty(gfxBmp))
        //    {
        //        gfxBmp.ExcludeClip(region);
        //        gfxBmp.Clear(Color.Transparent);
        //    }
        //    gfxBmp.Dispose();
        //    return bmp;
        //}

    }
}