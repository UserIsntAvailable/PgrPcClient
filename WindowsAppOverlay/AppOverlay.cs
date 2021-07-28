﻿using System;
using System.Runtime.InteropServices;
using static Win32Api.Window;
using static Win32Api.Error;
using static Win32Api.Message;
using static Win32Api.Resources;

namespace WindowsAppOverlay
{
    public class AppOverlay
    {
        /// <summary>
        /// This app handler pointer
        /// </summary>
        private nint _hWnd;

        private static IMessageHandler _messageHandler = new MessageHandler();

        private static readonly WndProc WndProcDelegate = WndProc;

        public AppOverlay(IMessageHandler messageHandler = null, string appName = "")
        {
            if(messageHandler != null)
            {
                _messageHandler = messageHandler;
            }

            if(RegisterClass(appName) && this.CreateWindow(appName)) return;

            // Something failed
            Console.WriteLine(GetLastError());
        }

        public void Run()
        {
            while(GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
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
                (uint) (WindowStyles.WS_VISIBLE | WindowStyles.WS_MAXIMIZE | WindowStyles.WS_POPUP),
                useDefault,
                useDefault,
                useDefault,
                useDefault,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            SetLayeredWindowAttributes(_hWnd, 0, 25, 0x00000002);

            return _hWnd != IntPtr.Zero;
        }

        private static bool RegisterClass(string className)
        {
            const int defaultResourceName = 32512;

            var wNdclass = new WNDCLASSEX
            {
                style = 0x8,
                cbSize = (uint) Marshal.SizeOf<WNDCLASSEX>(),
                lpfnWndProc = WndProcDelegate,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hIcon = LoadIconA(IntPtr.Zero, defaultResourceName),
                hCursor = LoadCursorA(IntPtr.Zero, defaultResourceName),
                hIconSm = IntPtr.Zero,
                hbrBackground = new IntPtr(6),
                lpszMenuName = null,
                lpszClassName = className,
            };

            if(RegisterClassExA(ref wNdclass) != 0) return true;

            Console.WriteLine($"Register Failed: ({GetLastError()})");

            return false;
        }

        private static nint WndProc(nint hWnd, uint message, nint wParam, nint lParam)
        {
            return _messageHandler.TryGetMessageDelegate(message, out var handleMessage)
                ? handleMessage(hWnd, wParam, lParam)
                : DefWindowProc(hWnd, message, wParam, lParam);
        }
    }
}