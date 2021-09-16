using System;
using static Win32Api.Message;

namespace PgrPcClientService
{
    public interface IWindowsMessageFaker
    {
        public Func<nint,uint,nint,nint,nint> MessageSender { get; }
        
        public nint KeyMessage(bool isKeyDown, nint wParam, nint lParam);

        public nint VirtualKeyMessage(nint vK, bool isKeyDown);
    }
}
