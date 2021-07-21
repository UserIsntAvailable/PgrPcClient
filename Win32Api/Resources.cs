using System;
using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Resources
    {
        #region Unmanaged
        #region Imports
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint LoadCursorA(nint hInstance, int lpCursorName);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint LoadIconA(nint hInstance, int lpIconName);
        #endregion
        #endregion
    }
}
