using System.Runtime.InteropServices;
using static Win32Api.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

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

        [DllImport("user32.dll")]
        public static extern nint GetDC(nint hdc);

        [DllImport("Gdi32.dll")]
        public static extern bool DeleteDC(nint hdc);

        [DllImport("Gdi32.dll")]
        public static extern bool DeleteObject(nint hdc);

        [DllImport("Gdi32.dll")]
        public static extern nint SelectObject(nint hdc, nint h);

        [DllImport("Gdi32.dll")]
        public static extern nint CreateCompatibleDC(nint hdc);

        [DllImport("Gdi32.dll")]
        public static extern nint CreateCompatibleBitmap(nint hdc, int cx, int cy);
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
        #endregion
        #endregion
    }
}
