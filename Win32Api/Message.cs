using System;
using static Win32Api.Common;
using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Message
    {
        #region Enums
        public enum WindowsMessage : uint
        {
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_PAINT = 0x000F,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MOUSEWHEEL = 0x020a,
            WM_XBUTTONDOWN = 0x020b,
            WM_XBUTTONUP = 0x020c,
        }
        #endregion

        #region Ummnaged
        #region Imports
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern sbyte GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint DispatchMessage(ref MSG lpmsg);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint SendMessage(nint hWnd, uint message, nint wParam, nint lParam);
        #endregion

        #region Structures
        public struct MSG
        {
            public nint hwnd;
            public uint message;
            public nint wParam;
            public nint lParam;
            public uint time;
            public POINT point;
            public uint lPrivate;
        }
        #endregion
        #endregion
    }
}
