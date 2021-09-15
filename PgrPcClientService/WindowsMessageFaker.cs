using System;
using System.Collections.Generic;
using Win32Api;
using static Win32Api.Message;

namespace PgrPcClientService
{
    public class WindowsMessageFaker : IWindowsMessageFaker
    {
        private readonly nint _appHWnd;
        private readonly IDictionary<nint, nint> _binds;

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

        public nint KeyMessage(WM wM, nint wParam, nint lParam) => throw new NotImplementedException();

        public nint VirtualKeyMessage(nint vK, WM wM) => throw new NotImplementedException();
    }
}
