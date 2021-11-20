using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;


namespace LoginMacroWPF
{
    public class ScreenCapture
    {
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //getting an scaling factor of monitor 1.25 = 125%
        public static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }
        //saving an image of application window
        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var scale = GetScalingFactor();
            while (!IsOpen(handle))
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle((int)(rect.Left * scale), (int)(rect.Top * scale), (int)((rect.Right - rect.Left) * scale), (int)((rect.Bottom - rect.Top) * scale));
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
        //if the application window is opened
        private static bool IsOpen(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            if (rect.Bottom == 0 && rect.Top == 0 && rect.Right == 0 && rect.Left == 0)
            {
                return false;
            }
            return true;
        }
    }
}
