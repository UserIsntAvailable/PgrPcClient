using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Win32Api
{
    public static class Window
    {
        #region Delegates
        public delegate nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam);

        public delegate bool EnumWindowsProc(nint hWnd, nint lParam);
        #endregion

        #region Enums
        public enum GWL
        {
            WNDPROC = -4,
            HINSTANCE = -6,
            HWNDPARENT = -8,
            STYLE = -16,
            EXSTYLE = -20,
            USERDATA = -21,
            ID = -12,
        }

        [Flags]
        public enum WS : uint
        {
            BORDER = 0x00800000,
            CAPTION = 0x00C00000,
            CHILD = 0x40000000,
            CHILDWINDOW = 0x40000000,
            CLIPCHILDREN = 0x02000000,
            CLIPSIBLINGS = 0x04000000,
            DISABLED = 0x08000000,
            DLGFRAME = 0x00400000,
            GROUP = 0x00020000,
            HSCROLL = 0x00100000,
            ICONIC = 0x20000000,
            MAXIMIZE = 0x01000000,
            MAXIMIZEBOX = 0x00010000,
            MINIMIZE = 0x20000000,
            MINIMIZEBOX = 0x00020000,
            OVERLAPPED = 0x00000000,
            OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
            POPUP = 0x80000000,
            POPUPWINDOW = POPUP | BORDER | SYSMENU,
            SIZEBOX = 0x00040000,
            SYSMENU = 0x00080000,
            TABSTOP = 0x00010000,
            THICKFRAME = 0x00040000,
            TILED = 0x00000000,
            TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
            VISIBLE = 0x10000000,
            VSCROLL = 0x00200000,
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Get the text for the window pointed to by hWnd
        /// </summary>
        public static string GetWindowText(nint hWnd)
        {
            var size = GetWindowTextLength(hWnd);

            if(size > 0)
            {
                var arrayPool = ArrayPool<char>.Shared;

                var sharedArray = arrayPool.Rent(size + 1);

                GetWindowText(hWnd, sharedArray, size + 1);

                var value = string.Concat(sharedArray);

                arrayPool.Return(sharedArray);

                return value;
            }

            return string.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter">
        ///     A delegate that returns true for windows
        ///     that should be returned and false for windows that should
        ///     not be returned
        /// </param>
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
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        public static extern nint SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(nint hWnd, char[] strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(nint hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, nint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern short RegisterClassExA(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll")]
        public static extern int GetWindowLongA(nint bShow, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLongA(nint bShow, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        public static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern nint CreateWindowExW(
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

        [DllImport("user32.dll", SetLastError = true)]
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
