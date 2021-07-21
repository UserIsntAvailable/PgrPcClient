using System.Runtime.InteropServices;

namespace Win32Api
{
    public class Error
    {
        #region Ummnaged
        #region Imports
        [DllImport("Kernel32.dll")]
        public static extern long GetLastError();
        #endregion
        #endregion
    }
}
