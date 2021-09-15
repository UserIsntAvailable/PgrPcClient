using System;
using static Win32Api.Message;

namespace PgrPcClientService
{
    public interface IWindowsMessageFaker
    {
        public Func<nint,uint,nint,nint,nint> MessageSender { get; set; }
        
        public nint KeyMessage(WM wM, nint hWnd, nint wParam, nint lParam);

        public nint VirtualKeyMessage(nint vK, nint hWnd, WM wM);
    }
}
