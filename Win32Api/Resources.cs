using System;
using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Resources
    {
        #region Unmanaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorA(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIconA(IntPtr hInstance, int lpIconName);
        #endregion
        #endregion
    }
}
