namespace Win32Api
{
    public static class Keyboard
    {
        public enum WindowsVirtualKeyCodes
        {
            Tilde = 0xC0,
            F = 0x46,
            O = 0x4F,
            P = 0x50,
            LeftBracket = 0xDB,
            RightBracket = 0xDD,
            M = 0x4D,
            SemiColon = 0xBA,
            Quote = 0xDE,
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
