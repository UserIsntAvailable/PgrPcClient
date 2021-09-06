using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Win32Api
{
    public static class Common
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public long x;
            public long y;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public long left;
            public long top;
            public long right;
            public long bottom;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public long cx;
            public long cy;
        }
    }
}
