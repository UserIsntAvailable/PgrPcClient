// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Win32Api
{
    public static class Macros
    {
        public static ushort HIWORD(nint value) => (ushort)(value >> 16);
    }
}
