// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Win32Api
{
    public static class Macros
    {
        public static int GET_X_LPARAM(int lp) => (short) LOWORD(lp);

        public static int GET_Y_LPARAM(int lp) => (short) HIWORD(lp);

        public static ushort HIWORD(nint value) => (ushort) (value >> 16);

        public static ushort LOWORD(nint value) => (ushort) (value & 0xffff);
    }
}
