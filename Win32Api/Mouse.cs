using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo

namespace Win32Api
{
    public static class Mouse
    {
        #region Ummnaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern int GET_X_LPARAM(int lp);

        [DllImport("user32.dll")]
        public static extern int GET_Y_LPARAM(int lp);

        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Common.POINT p);
        #endregion
        #endregion
    }
}
