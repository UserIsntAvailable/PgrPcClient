// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Win32Api
{
    public static class Keyboard
    {
        // I'm not including all of them
        public enum VK : byte
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            MBUTTON = 0x04,
            XBUTTON1 = 0x05,
            XBUTTON2 = 0x06,
            
            // TODO - Add the extra Virtual Keys
            Key_0 = 0x30,
        }

        public enum KeyScanCode : ushort
        {
            O = 0x18,
            P = 0x19,
            F = 0x21,
            LeftBracket = 0x1A,
            RightBracket = 0x1B,
            M = 0x32,
            SemiColon = 0x27,
            Quote = 0x28,
        }
    }
}
