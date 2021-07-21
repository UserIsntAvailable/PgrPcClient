using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Mouse
    {
        #region Ummnaged
        #region Imports
        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);
        
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Common.POINT p);
        #endregion

        #region Structures
        
        #endregion
        #endregion
    }
}
