using System;
using System.Collections.Generic;
using static Win32Api.Message;
using static Win32Api.Keyboard;

namespace PgrPcClientService
{
    public class WindowsMessageFaker : IWindowsMessageFaker
    {
        private readonly nint _appHWnd;
        private readonly IDictionary<nint, nint> _binds;
        private readonly nint _currentKeyboardLayout = GetKeyboardLayout(0);

        public WindowsMessageFaker(
            nint appHWnd,
            IDictionary<nint, nint> binds,
            Func<nint, uint, nint, nint, nint> messageSender)
        {
            _appHWnd = appHWnd;
            _binds = binds;
            this.MessageSender = messageSender;
        }

        public Func<nint, uint, nint, nint, nint> MessageSender { get; set; }

        public nint KeyMessage(WM wM, nint wParam, nint lParam)
        {
            var message = (uint) wM;

            return _binds.ContainsKey(wParam)
                ? this.VirtualKeyMessage(wParam, wM)
                : this.MessageSender(_appHWnd, message, wParam, lParam);
        }

        public nint VirtualKeyMessage(nint vK, WM wM)
        {
            var message = (uint) wM;

            if(!_binds.TryGetValue(vK, out var value)) return 0;

            var scanCode = MapVirtualKeyExA((uint) value, (uint) MAPVK.VK_TO_VSC, _currentKeyboardLayout);

            var newLParam = FakeKeyLParam(scanCode, wM == WM.KEYDOWN);

            return this.MessageSender(_appHWnd, message, value, newLParam);

            static nint FakeKeyLParam(nint scanCode, bool isKeyDown)
            {
                const int repeat = 1;

                var scancode = (int) scanCode << 16;
                var downOrUpFlag = isKeyDown ? 0 : 3 << 30;

                return repeat + scancode + downOrUpFlag;
            }
        }
    }
}
