using System.Runtime.InteropServices;

namespace Win32Api
{
    public static class Error
    {
        #region Ummnaged
        #region Imports
        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();
        #endregion
        #endregion
    }
}
