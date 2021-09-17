using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AdbMouseFaker;
using Microsoft.Extensions.Configuration;
using PgrPcClientService.Extensions;
using WindowsAppOverlay;
using static Win32Api.Message;
using static Win32Api.Window;
using static Win32Api.Mouse;
using static Win32Api.Keyboard;
using static Win32Api.Macros;
using static Win32Api.WinGdi;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PgrPcClientService
{
    public class PGRMessageHandler : IMessageHandler
    {
        private const int VK_MWHEELUP = 0x0E;
        private const int VK_MWHEELDOWN = 0x0F;

        private readonly IMouseFaker _mouseFaker;
        private readonly IWindowsMessageFaker _messageFaker;
        private readonly ReadOnlyDictionary<uint, MessageHandler.HandleMessage> _messageHooks;

        private readonly int _screenWidth = GetSystemMetrics(0);
        private readonly int _screenHeight = GetSystemMetrics(1);

        private bool _winCreated;

        /*
         * TODO - Implement auto reloading of the appsettings.json
         * TODO - Focus overlay when PGR is opened
         * TODO - Bind (-/+) to change the alpha value of the overlay
         */
        public PGRMessageHandler(IMouseFaker mouseFaker, IWindowsMessageFaker messageFaker, IConfiguration config)
        {
            _mouseFaker = mouseFaker;
            _messageFaker = messageFaker;

            // TODO - Create method attribute to auto parse Handle message delegates
            var dict = new Dictionary<uint, MessageHandler.HandleMessage>
            {
                {
                    // TODO - Refactor all this mess
                    (uint)WM.CREATE, (hWnd, _, lParam) =>
                    {
                        if(_winCreated) return 0;

                        _winCreated = true;

                        var keys = config.GetSection("OverlayBindings").GetChildren()
                                         .Where(child => child.Value.StartsWith("Signal")).OrderBy(child => child.Value)
                                         .Select(
                                             child =>
                                                 // TODO - Refactor
                                             {
                                                 var value = child.Key;

                                                 if(value.Length == 1) return value;

                                                 if(value.IsHexValue())
                                                 {
                                                     return StrToNint(value) == VK_MWHEELUP ? "WU" : "WD";
                                                 }

                                                 if(value.StartsWith("XB"))
                                                 {
                                                     return$"XB{value[^1]}";
                                                 }

                                                 return value[..2];
                                             }
                                         );

                        CreateKeymapsHelperWindow(
                            hWnd,
                            config["AppClassName"],
                            keys,
                            (int.Parse(config["KeyBindsWindowSettings:xStartPosition"]),
                             int.Parse(config["KeyBindsWindowSettings:yStartPosition"])),
                            int.Parse(config["KeyBindsWindowSettings:Padding"])
                        );

                        return 0;
                    }
                },
                { (uint)WM.DESTROY, this.OnDestroy },
                { (uint)WM.KEYDOWN, this.OnKeyPressed },
                { (uint)WM.KEYUP, this.OnKeyReleased },
                { (uint)WM.MOUSEMOVE, this.OnMouseMove },
                { (uint)WM.LBUTTONDOWN, this.OnLMButtonPressed },
                { (uint)WM.LBUTTONUP, this.OnLMButtonReleased },
                // TODO - LBUTTONDBLCLK/RBUTTONDBLCLK events are problematic
                { (uint)WM.LBUTTONDBLCLK, this.OnLMButtonPressed },
                { (uint)WM.RBUTTONDBLCLK, this.OnRMButtonPressed },
                //
                { (uint)WM.RBUTTONDOWN, this.OnRMButtonPressed },
                { (uint)WM.RBUTTONUP, this.OnRMButtonReleased },
                { (uint)WM.XBUTTONDOWN, this.OnXMButtonPressed },
                { (uint)WM.XBUTTONUP, this.OnXMButtonReleased },
                { (uint)WM.MOUSEWHEEL, this.OnMouseWheel },
            };

            _messageHooks = new ReadOnlyDictionary<uint, MessageHandler.HandleMessage>(dict);
        }

        public bool TryGetMessageDelegate(uint message, out MessageHandler.HandleMessage @delegate)
        {
            if(_messageHooks.TryGetValue(message, out var handleMessage))
            {
                @delegate = handleMessage;

                return true;
            }

            @delegate = null;

            return false;
        }

        // TODO - Move handle messages delegates to their own class

        #region HandleMessage Delegates
        private nint OnDestroy(nint hWnd, nint wParam, nint lParam)
        {
            _mouseFaker.IsCameraModeOn = false;

            PostQuitMessage(0);

            return 0;
        }

        private nint OnKeyPressed(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.KeyMessage(true, wParam, lParam);

        private nint OnKeyReleased(nint hWnd, nint wParam, nint lParam)
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

        private nint OnMouseMove(nint hWnd, nint wParam, nint lParam)
        {
            const int PADDING = 1;

            var xPos = GET_X_LPARAM((int)lParam);
            var yPos = GET_Y_LPARAM((int)lParam);

            if(xPos == _screenWidth - PADDING)
            {
                SetCursorPos(PADDING, yPos);
            }
            else if(xPos == 0)
            {
                SetCursorPos(_screenWidth - PADDING - 1, yPos);
            }

            return 0;
        }

        private nint OnLMButtonPressed(nint hWnd, nint wParam, nint lParam)
        {
            if(!_mouseFaker.IsCameraModeOn)
            {
                // TODO - Be able to drag
                _mouseFaker.Click(GET_X_LPARAM((int)lParam), GET_Y_LPARAM((int)lParam));

                return 0;
            }

            return _messageFaker.VirtualKeyMessage((int)VK.LBUTTON, true);
        }

        private nint OnLMButtonReleased(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.LBUTTON, false);

        private nint OnRMButtonPressed(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.RBUTTON, true);

        private nint OnRMButtonReleased(nint hWnd, nint wParam, nint lParam) =>
            _messageFaker.VirtualKeyMessage((int)VK.RBUTTON, false);

        private nint OnXMButtonPressed(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return _messageFaker.VirtualKeyMessage(vK, true);
        }

        private nint OnXMButtonReleased(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return _messageFaker.VirtualKeyMessage(vK, false);
        }

        private nint OnMouseWheel(nint hWnd, nint wParam, nint lParam)
        {
            if(IsMWheelGoingUp(wParam))
            {
                _messageFaker.VirtualKeyMessage(VK_MWHEELUP, true);

                return _messageFaker.VirtualKeyMessage(VK_MWHEELUP, false);
            }

            _messageFaker.VirtualKeyMessage(VK_MWHEELDOWN, true);

            return _messageFaker.VirtualKeyMessage(VK_MWHEELDOWN, false);
        }
        #endregion

        #region Helper Methods
        /*
         * TODO - Move CreateKeymapsHelperWindow to its own class
         * TODO - GetClass of parentHWnd and modify lpfnWndProc
         * TODO - Fix Anti Aliassing of text
         * TODO - Be able to configure parameters of DrawString ( Font, Color, etc )
         */
        private static nint CreateKeymapsHelperWindow(
            nint parentHWnd,
            string appClassName,
            IEnumerable<string> keysToDraw,
            (int X, int Y) startPosition,
            int padding)
        {
            var cHWnd = CreateWindowExW(
                0x00080000,
                appClassName,
                null,
                (uint)(WS.VISIBLE | WS.MAXIMIZE | WS.POPUP),
                0,
                0,
                0,
                0,
                parentHWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            SetLayeredWindowAttributes(cHWnd, (uint)RGB(255, 255, 255), 255, 1);

            PAINTSTRUCT ps = new();
            var hdc = BeginPaint(cHWnd, ps);

            using(var graphics = Graphics.FromHdc(hdc))
            {
                foreach(var key in keysToDraw)
                {
                    graphics.DrawString(
                        key,
                        new Font("Arial", 14),
                        new SolidBrush(Color.Red),
                        new PointF(startPosition.X, startPosition.Y)
                    );

                    startPosition.X -= padding;
                }
            }

            EndPaint(hdc, ref ps);

            return cHWnd;
        }

        // TODO - I need to remove this ( I will do it once I refactor all the class ).
        private static nint StrToNint(string str)
        {
            if(str.IsHexValue()) return nint.Parse(str[2..], NumberStyles.HexNumber);

            if(Enum.IsDefined(typeof(VK), str))
            {
                return(nint)(uint)Enum.Parse(typeof(VK), str, true);
            }

            return char.Parse(str);
        }

        private static int GetXButtonVirtualKey(nint wParam) => (int)VK.MBUTTON + HIWORD(wParam);

        private static bool IsMWheelGoingUp(nint wParam) => (short)HIWORD(wParam) > 0;
        #endregion
    }
}
