using AdbMouseFaker;
using WindowsAppOverlay;
using static Win32Api.Message;
using static Win32Api.Window;
using static Win32Api.Mouse;
using static Win32Api.Keyboard;
using static Win32Api.Macros;

// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global

namespace PgrPcClient
{
    /*
     * TODO - Focus overlay when PGR is opened
     * TODO - Bind (-/+) to change the alpha value of the overlay
     */
    public class HandleMessageMapper
    {
        private const int VK_MWHEELUP = 0x0E;
        private const int VK_MWHEELDOWN = 0x0F;

        private readonly IMouseFaker _mouseFaker;
        private readonly IWindowsMessageFaker _messageFaker;

        private readonly int _screenWidth = GetSystemMetrics(0);
        private readonly int _screenHeight = GetSystemMetrics(1);

        public HandleMessageMapper(
            IMessageHandler messageHandler,
            IWindowsMessageFaker messageFaker,
            IMouseFaker mouseFaker)
        {
            _messageFaker = messageFaker;
            _mouseFaker = mouseFaker;

            // TODO - Map the helper window ( Removing it for now, until I refactor it )
            messageHandler.Map(WM.DESTROY, this.OnDestroy);
            messageHandler.Map(WM.KEYDOWN, this.OnKeyDown);
            messageHandler.Map(WM.KEYUP, this.OnKeyUp);
            messageHandler.Map(WM.MOUSEMOVE, this.OnMouseMove);
            messageHandler.Map(WM.LBUTTONDOWN, this.OnLMButtonDown);
            messageHandler.Map(WM.LBUTTONUP, this.OnLMButtonUp);
            messageHandler.Map(WM.RBUTTONDOWN, this.OnRMButtonDown);
            messageHandler.Map(WM.RBUTTONUP, this.OnRMButtonUp);
            messageHandler.Map(WM.XBUTTONDOWN, this.OnXMButtonDown);
            messageHandler.Map(WM.XBUTTONUP, this.OnXMButtonUp);
            messageHandler.Map(WM.MOUSEWHEEL, this.OnMouseWheel);
        }

        #region Handle Messages
        internal nint OnDestroy(nint hWnd, nint wParam, nint lParam)
        {
            _mouseFaker.IsCameraModeOn = false;

            PostQuitMessage(0);

            return 0;
        }

        internal nint OnKeyDown(nint hWnd, nint wParam, nint lParam) => _messageFaker.KeyMessage(true, wParam, lParam);

        internal nint OnKeyUp(nint hWnd, nint wParam, nint lParam)
        {
            switch(wParam)
            {
                // TODO - Set CameraMode to true automatically when entering a stage
                case'R':
                {
                    ShowCursor(_mouseFaker.IsCameraModeOn);
                    SetCursorPos(_screenWidth / 2, _screenHeight / 2);
                    _mouseFaker.IsCameraModeOn = !_mouseFaker.IsCameraModeOn;

                    return 0;
                }
                default:
                    return _messageFaker.KeyMessage(false, wParam, lParam);
            }
        }

        internal nint OnMouseMove(nint hWnd, nint wParam, nint lParam)
        {
            const int padding = 1;

            var xPos = GET_X_LPARAM((int)lParam);
            var yPos = GET_Y_LPARAM((int)lParam);

            if(xPos == _screenWidth - padding)
            {
                SetCursorPos(padding, yPos);
            }
            else if(xPos == 0)
            {
                SetCursorPos(_screenWidth - padding - 1, yPos);
            }

            return 0;
        }

        internal nint OnLMButtonDown(nint hWnd, nint wParam, nint lParam)
        {
            if(!_mouseFaker.IsCameraModeOn)
            {
                // TODO - Be able to drag
                _mouseFaker.Click(GET_X_LPARAM((int)lParam), GET_Y_LPARAM((int)lParam));

                return 0;
            }

            return _messageFaker.VirtualKeyMessage((int)VK.LBUTTON, true);
        }

        internal nint OnLMButtonUp(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.LBUTTON, false);

        internal nint OnRMButtonDown(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.RBUTTON, true);

        internal nint OnRMButtonUp(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.RBUTTON, false);

        internal nint OnXMButtonDown(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return _messageFaker.VirtualKeyMessage(vK, true);
        }

        internal nint OnXMButtonUp(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return _messageFaker.VirtualKeyMessage(vK, false);
        }

        internal nint OnMouseWheel(nint hWnd, nint wParam, nint lParam)
        {
            if((short)HIWORD(wParam) > 0) // Is mouse wheel going up
            {
                _messageFaker.VirtualKeyMessage(VK_MWHEELUP, true);

                return _messageFaker.VirtualKeyMessage(VK_MWHEELUP, false);
            }

            _messageFaker.VirtualKeyMessage(VK_MWHEELDOWN, true);

            return _messageFaker.VirtualKeyMessage(VK_MWHEELDOWN, false);
        }
        #endregion

        #region Helper Methods
        private static int GetXButtonVirtualKey(nint wParam) => (int)VK.MBUTTON + HIWORD(wParam);
        #endregion
    }
}
