using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Common
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct POINT
        {
            public readonly int X;
            public readonly int Y;
        }
    }
}
