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
        private static extern nint GetDC(nint hdc);

        [DllImport("Gdi32.dll")]
        private static extern bool DeleteDC(nint hdc);

        [DllImport("Gdi32.dll")]
        private static extern bool DeleteObject(nint hdc);

        [DllImport("Gdi32.dll")]
        private static extern nint SelectObject(nint hdc, nint h);

        [DllImport("Gdi32.dll")]
        private static extern nint CreateCompatibleDC(nint hdc);

        [DllImport("Gdi32.dll")]
        private static extern nint CreateCompatibleBitmap(nint hdc, int cx, int cy);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern int UpdateLayeredWindow(
            nint hwnd,
            nint hdcDst,
            out POINT pptDst,
            out SIZE psize,
            nint hdcSrc,
            out POINT pptSrc,
            uint crKey,
            [In] ref BLENDFUNCTION pblend,
            uint dwFlags);
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
        private struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }
        #endregion
        #endregion
    }
}
