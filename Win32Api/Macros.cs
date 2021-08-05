// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Win32Api
{
    public static class Macros
    {
        public static nint HIWORD(nint value) => value >> 16;
    }
}
