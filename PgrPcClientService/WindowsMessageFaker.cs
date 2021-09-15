using System;
using System.Collections.Generic;
using Win32Api;
using static Win32Api.Message;

namespace PgrPcClientService
{
    public class WindowsMessageFaker : IWindowsMessageFaker
    {
        private readonly IDictionary<nint, nint> _binds;

        public WindowsMessageFaker(IDictionary<nint, nint> binds, Func<nint, uint, nint, nint, nint> messageSender)
        {
            _binds = binds;
            this.MessageSender = messageSender;
        }

        public Func<nint, uint, nint, nint, nint> MessageSender { get; set; }

        public nint KeyMessage(WM wM, nint hWnd, nint wParam, nint lParam) => throw new NotImplementedException();

        public nint VirtualKeyMessage(nint vK, nint hWnd, Message.WM wM) => throw new NotImplementedException();
    }
}
