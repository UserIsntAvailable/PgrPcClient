using System.Runtime.InteropServices;
using static Win32Api.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Win32Api
{
    public static class Message
    {
        #region Enums
        public enum WM : uint
        {
            CREATE = 0x0001,
            DESTROY = 0x0002,
            PAINT = 0x000F,
            KEYDOWN = 0x0100,
            KEYUP = 0x0101,
            MOUSEMOVE = 0x0200,
            LBUTTONDOWN = 0x0201,
            LBUTTONUP = 0x0202,
            LBUTTONDBLCLK = 0x0203,
            RBUTTONDOWN = 0x0204,
            RBUTTONUP = 0x0205,
            RBUTTONDBLCLK = 0x0206,
            MBUTTONDOWN = 0x0207,
            MBUTTONUP = 0x0208,
            MOUSEWHEEL = 0x020A,
            XBUTTONDOWN = 0x020B,
            XBUTTONUP = 0x020C,
        }
        #endregion

        #region Ummnaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern sbyte GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern nint DispatchMessage(ref MSG lpmsg);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
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
