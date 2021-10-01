using System;
using System.Runtime.InteropServices;
using static Win32Api.Window;
using static Win32Api.Error;
using static Win32Api.Message;

namespace WindowsAppOverlay
{
    public class AppOverlay
    {
        private static IMessageHandler _messageHandler;

        /// <summary>
        /// This app handler pointer
        /// </summary>
        private nint _hWnd;

        public AppOverlay(IMessageHandler messageHandler, string appName)
        {
            _messageHandler = messageHandler;

            if(RegisterClass(appName) && this.CreateWindow(appName)) return;

            // Something failed
            Console.WriteLine(GetLastError());
        }

        public void Run()
        {
            while(GetMessage(out var msg, 0, 0, 0) > 0)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
        }

        private bool CreateWindow(string className)
        {
            const int useDefault = 250;
            _hWnd = CreateWindowExW(
                0x00080000,
                className,
                null,
                (uint)(WS.VISIBLE | WS.MAXIMIZE | WS.POPUP),
                useDefault,
                useDefault,
                useDefault,
                useDefault,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            SetLayeredWindowAttributes(_hWnd, 0, 1, 0x00000002);

            return _hWnd != IntPtr.Zero;
        }

        private static unsafe bool RegisterClass(string className)
        {
            var wNdclass = new WNDCLASSEX(
                className,
                lpfnWndProc: &WndProc,
                hbrBackground: 6
            );

            if(RegisterClassExA(ref wNdclass) != 0) return true;

            Console.WriteLine($"Register Failed: ({GetLastError()})");

            return false;
        }
        
        [UnmanagedCallersOnly]
        private static nint WndProc(nint hWnd, uint message, nint wParam, nint lParam) =>
            _messageHandler.TryGetMessageDelegate(message, out var handleMessage)
                ? handleMessage(hWnd, wParam, lParam)
                : DefWindowProc(hWnd, message, wParam, lParam);
    }
}
