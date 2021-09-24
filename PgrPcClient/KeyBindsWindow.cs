using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using static Win32Api.Window;
using static Win32Api.WinGdi;
using static Win32Api.Macros;

namespace PgrPcClient
{
    public static class KeyBindsWindow
    {
        /*
         * TODO - Move CreateKeymapsHelperWindow to its own class
         * TODO - Fix Anti Aliasing of text
         * TODO - Be able to configure parameters of DrawString ( Font, Color, etc )
         */
        public static nint CreateKeyBindsChildWindow(
            nint parentHWnd,
            IEnumerable<string> keysToDraw,
            (int X, int Y) startPosition,
            int padding)
        {
            var cHWnd = CreateWindowExW(
                0x00080000,
                RegisterChildWndClass(parentHWnd),
                null,
                (uint) (WS.VISIBLE | WS.MAXIMIZE | WS.POPUP),
                0,
                0,
                0,
                0,
                parentHWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            SetLayeredWindowAttributes(cHWnd, (uint) RGB(255, 255, 255), 255, 1);

            DrawKeyBinds(cHWnd, keysToDraw, startPosition, padding);
            
            return cHWnd;

            static string RegisterChildWndClass(nint parentHWnd)
            {
                var parentWndClassEx = GetParentWindowClassInfo(parentHWnd);

                var childLpszClassName = $"child:{parentWndClassEx.lpszClassName}";

                parentWndClassEx.lpszClassName = childLpszClassName;
                parentWndClassEx.lpfnWndProc = DefWindowProc;

                if(RegisterClassExA(ref parentWndClassEx) == 0)
                {
                    throw new Exception("RegisterClassExA failed.");
                }

                return childLpszClassName;
            }

            static void DrawKeyBinds(
                nint hWnd,
                IEnumerable<string> keysToDraw,
                (int X, int Y) startPosition,
                int padding)
            {
                PAINTSTRUCT ps = new();
                var hdc = BeginPaint(hWnd, ps);

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
            }

            static WNDCLASSEX GetParentWindowClassInfo(nint parentHWnd)
            {
                const int maxLpszClassNameLength = 256;

                var arrayPool = ArrayPool<char>.Shared;

                var sharedArray = arrayPool.Rent(maxLpszClassNameLength);

                if(GetClassName(parentHWnd, sharedArray, maxLpszClassNameLength) == 0)
                {
                    throw new Exception("GetClassName failed.");
                }

                var lpszClassName = string.Concat(sharedArray);

                arrayPool.Return(sharedArray);

                if(!GetClassInfoExW(0, lpszClassName, out var lpwcz))
                {
                    throw new Exception("GetClassInfoExW failed.");
                }

                return lpwcz;
            }
        }
    }
}
