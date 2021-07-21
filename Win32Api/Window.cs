using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Win32Api
{
    public static class Window
    {
        #region Delegates
        public delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

        public delegate bool EnumWindowsProc(nint hWnd, nint lParam);
        #endregion

        #region Enums
        public enum GetWindowLong
        {
            GWL_WNDPROC = -4,
            GWL_HINSTANCE = -6,
            GWL_HWNDPARENT = -8,
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
            GWL_USERDATA = -21,
            GWL_ID = -12,
        }

        [Flags]
        public enum WindowStyles : uint
        {
            WS_BORDER = 0x00800000,
            WS_CAPTION = 0x00C00000,
            WS_CHILD = 0x40000000,
            WS_CHILDWINDOW = 0x40000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_DISABLED = 0x08000000,
            WS_DLGFRAME = 0x00400000,
            WS_GROUP = 0x00020000,
            WS_HSCROLL = 0x00100000,
            WS_ICONIC = 0x20000000,
            WS_MAXIMIZE = 0x01000000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_OVERLAPPED = 0x00000000,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX |
                                  WS_MAXIMIZEBOX,
            WS_POPUP = 0x80000000,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEBOX = 0x00040000,
            WS_SYSMENU = 0x00080000,
            WS_TABSTOP = 0x00010000,
            WS_THICKFRAME = 0x00040000,
            WS_TILED = 0x00000000,
            WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x00200000,
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the text for the window pointed to by hWnd
        /// </summary>
        public static string GetWindowText(nint hWnd)
        {
            int size = GetWindowTextLength(hWnd);

            if(size > 0)
            {
                var arrayPool = ArrayPool<char>.Shared;

                var sharedArray = arrayPool.Rent(size + 1);

                GetWindowText(hWnd, sharedArray, size + 1);

                var value = string.Concat(arrayPool);

                arrayPool.Return(sharedArray);

                return value;
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static IEnumerable<nint> FindWindows(EnumWindowsProc filter)
        {
            nint found = IntPtr.Zero;
            var windows = new List<nint>();

            EnumWindows(
                delegate(nint wnd, nint param)
                {
                    if(filter(wnd, param))
                    {
                        // only add the windows that pass the filter
                        windows.Add(wnd);
                    }

                    // but return true here so that we iterate all windows
                    return true;
                },
                IntPtr.Zero
            );

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<nint> FindWindowsWithText(string titleText)
        {
            return FindWindows((wnd, param) => GetWindowText(wnd).Contains(titleText));
        }
        #endregion

        #region Ummnaged
        #region Imports
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(nint hWnd, char[] strText, int maxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(nint hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, nint lParam);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern short RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int GetWindowLongA(nint bShow, int nIndex);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int SetWindowLongA(nint bShow, int nIndex, long dwNewLong);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern nint CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            nint hWndParent,
            nint hMenu,
            nint hInstance,
            nint lpParam);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public nint hInstance;
            public nint hIcon;
            public nint hCursor;
            public nint hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public nint hIconSm;
        }
        #endregion
        #endregion
    }
}
