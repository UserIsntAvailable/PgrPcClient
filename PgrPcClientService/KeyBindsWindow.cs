using System;
using System.Collections.Generic;
using System.Drawing;
using Win32Api;
using static Win32Api.Window;
using static Win32Api.WinGdi;
using static Win32Api.Macros;

namespace PgrPcClientService
{
    public static class KeyBindsWindow
    {
        /*
         * TODO - Move CreateKeymapsHelperWindow to its own class
         * TODO - GetClass of parentHWnd and modify lpfnWndProc
         * TODO - Fix Anti Aliasing of text
         * TODO - Be able to configure parameters of DrawString ( Font, Color, etc )
         */
        public static nint CreateKeymapsHelperWindow(
            nint parentHWnd,
            string appClassName,
            IEnumerable<string> keysToDraw,
            (int X, int Y) startPosition,
            int padding)
        {
            var cHWnd = CreateWindowExW(
                0x00080000,
                appClassName,
                null,
                (uint)(Window.WS.VISIBLE | Window.WS.MAXIMIZE | Window.WS.POPUP),
                0,
                0,
                0,
                0,
                parentHWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            SetLayeredWindowAttributes(cHWnd, (uint)RGB(255, 255, 255), 255, 1);

            WinGdi.PAINTSTRUCT ps = new();
            var hdc = BeginPaint(cHWnd, ps);

            using(var graphics = Graphics.FromHdc(hdc))
            {
                foreach(var key in keysToDraw)
                {
                    graphics.DrawString(
                        key,
                        new Font("Arial", 14),
                        new SolidBrush(Color.Red),
                        new PointF(startPosition.X, startPosition.Y)
                    );

                    startPosition.X -= padding;
                }
            }

            EndPaint(hdc, ref ps);

            return cHWnd;
        }
    }
}
