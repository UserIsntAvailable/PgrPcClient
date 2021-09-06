using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class WinGdi
    {
        #region Ummnaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern nint BeginPaint(nint nWnd, PAINTSTRUCT lpPaint);
        
        [DllImport("user32.dll")]
        public static extern bool EndPaint(nint hWnd, [In] ref PAINTSTRUCT lpPaint);
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public nint hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public long left;
            public long top;
            public long right;
            public long bottom;
        }
        #endregion
        #endregion
    }
}
