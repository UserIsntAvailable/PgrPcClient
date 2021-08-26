// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Keyboard
    {
        #region Enums
        public enum VK : uint
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            MBUTTON = 0x04,
            XBUTTON1 = 0x05,
            XBUTTON2 = 0x06,
            
            SPACE = 0x20,

            LSHIFT = 0xA0,
            RSHIFT = 0xA1,
        }
        #endregion

        #region Ummnaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern nint MapVirtualKeyExA(uint uCode, uint uMapType, nint dwnkl);

        [DllImport("user32.dll")]
        public static extern nint GetKeyboardLayout(ulong idThread);
        #endregion
        #endregion
    }
}
