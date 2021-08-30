﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using WindowsAppOverlay;
using AdbMouseFaker;
using Microsoft.Extensions.Configuration;
using static Win32Api.Message;
using static Win32Api.Window;
using static Win32Api.Mouse;
using static Win32Api.Keyboard;
using static Win32Api.Macros;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace PgrPcClientService
{
    public class PGRMessageHandler : IMessageHandler
    {
        private const int VK_MWHEELUP = 0x0E;
        private const int VK_MWHEELDOWN = 0x0F;
        private const int MAPVK_VK_TO_VSC = 0;

        private readonly IMouseFaker _mouseFaker;
        private readonly nint _appToHook;
        private readonly Dictionary<nint, nint> _binds = new();
        private readonly ReadOnlyDictionary<uint, MessageHandler.HandleMessage> _messageHooks;

        private readonly int _screenWidth = GetSystemMetrics(0);
        private readonly int _screenHeight = GetSystemMetrics(1);
        private readonly nint _currrentKeyboardLayout = GetKeyboardLayout(0);

        /*
         * TODO - Implement auto reloading of the appsettings.json
         * TODO - Focus overlay when PGR is opened
         * TODO - Create help menu ( it just display what keys are bind to what )
         * TODO - Bind (-/+) to change the alpha value of the overlay
         */
        public PGRMessageHandler(IMouseFaker mouseFaker, IConfiguration config)
        {
            _mouseFaker = mouseFaker;
            _appToHook = nint.Parse(config["AppToHook"]);

            var gKbSection = config.GetSection("GameKeyBindings");

            foreach(var children in config.GetSection("OverlayBinds").GetChildren())
            {
                var keyValue = StrToNint(children.Key);

                var valueValue =
                    gKbSection.Exists() ? StrToNint(gKbSection[children.Value]) : StrToNint(children.Value);

                _binds.Add(keyValue, valueValue);
            }

            // TODO - Create method attribute to auto parse Handle message delegates
            var dict = new Dictionary<uint, MessageHandler.HandleMessage>
            {
                {
                    (uint) VM.DESTROY, (_, _, _) =>
                    {
                        _mouseFaker.IsCameraModeOn = false;
                        
                        PostQuitMessage(0);

                        return 0;
                    }
                },
                {(uint) VM.KEYDOWN, this.OnKeyPressed},
                {(uint) VM.KEYUP, this.OnKeyReleased},
                {(uint) VM.MOUSEMOVE, this.OnMouseMove},
                {(uint) VM.LBUTTONDOWN, this.OnLMButtonPressed},
                {(uint) VM.LBUTTONUP, this.OnLMButtonReleased},
                // TODO - LBUTTONDBLCLK/RBUTTONDBLCLK events are problematic
                {(uint) VM.LBUTTONDBLCLK, this.OnLMButtonPressed},
                {(uint) VM.RBUTTONDBLCLK, this.OnRMButtonPressed},
                //
                {(uint) VM.RBUTTONDOWN, this.OnRMButtonPressed},
                {(uint) VM.RBUTTONUP, this.OnRMButtonReleased},
                {(uint) VM.XBUTTONDOWN, this.OnXMButtonPressed},
                {(uint) VM.XBUTTONUP, this.OnXMButtonReleased},
                {(uint) VM.MOUSEWHEEL, this.OnMouseWheel},
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

        #region HandleMessage Delegates
        private nint OnKeyPressed(nint hWnd, nint wParam, nint lParam) => this.KeyMessage(VM.KEYDOWN, wParam, lParam);

        private nint OnKeyReleased(nint hWnd, nint wParam, nint lParam)
        {
            switch(wParam)
            {
                case'R':
                {
                    ShowCursor(_mouseFaker.IsCameraModeOn);
                    SetCursorPos(_screenWidth / 2, _screenHeight / 2);
                    _mouseFaker.IsCameraModeOn = !_mouseFaker.IsCameraModeOn;

                    return 0;
                }
                default:
                    return this.KeyMessage(VM.KEYUP, wParam, lParam);
            }
        }

        private nint OnMouseMove(nint hWnd, nint wParam, nint lParam)
        {
            const int PADDING = 1;

            var xPos = GET_X_LPARAM((int) lParam);
            var yPos = GET_Y_LPARAM((int) lParam);

            if(xPos == _screenWidth - PADDING)
                SetCursorPos(PADDING, yPos);
            else if(xPos == 0) SetCursorPos(_screenWidth - PADDING - 1, yPos);

            return 0;
        }

        private nint OnLMButtonPressed(nint hWnd, nint wParam, nint lParam)
        {
            if(!_mouseFaker.IsCameraModeOn)
            {
                _mouseFaker.Click(GET_X_LPARAM((int) lParam), GET_Y_LPARAM((int) lParam));

                return 0;
            }

            return this.FakeVirtualKeyMessage((int) VK.LBUTTON, VM.KEYDOWN);
        }

        private nint OnLMButtonReleased(nint hWnd, nint wParam, nint lParam) =>
            this.FakeVirtualKeyMessage((int) VK.LBUTTON, VM.KEYUP);

        private nint OnRMButtonPressed(nint hWnd, nint wParam, nint lParam) =>
            this.FakeVirtualKeyMessage((int) VK.RBUTTON, VM.KEYDOWN);

        private nint OnRMButtonReleased(nint hWnd, nint wParam, nint lParam) =>
            this.FakeVirtualKeyMessage((int) VK.RBUTTON, VM.KEYUP);

        private nint OnXMButtonPressed(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return this.FakeVirtualKeyMessage(vK, VM.KEYDOWN);
        }

        private nint OnXMButtonReleased(nint hWnd, nint wParam, nint lParam)
        {
            var vK = GetXButtonVirtualKey(wParam);

            return this.FakeVirtualKeyMessage(vK, VM.KEYUP);
        }

        private nint OnMouseWheel(nint hWnd, nint wParam, nint lParam)
        {
            if(IsMWheelGoingUp(wParam))
            {
                this.FakeVirtualKeyMessage(VK_MWHEELUP, VM.KEYDOWN);

                return this.FakeVirtualKeyMessage(VK_MWHEELUP, VM.KEYUP);
            }

            this.FakeVirtualKeyMessage(VK_MWHEELDOWN, VM.KEYDOWN);

            return this.FakeVirtualKeyMessage(VK_MWHEELDOWN, VM.KEYUP);
        }
        #endregion

        #region Private Methods
        private nint KeyMessage(VM vM, nint wParam, nint lParam)
        {
            var message = (uint) vM;

            return _binds.ContainsKey(wParam)
                ? this.FakeVirtualKeyMessage(wParam, vM)
                : SendMessage(_appToHook, message, wParam, lParam);
        }

        private nint FakeVirtualKeyMessage(nint vK, VM vM)
        {
            var message = (uint) vM;

            if(_binds.TryGetValue(vK, out var value))
            {
                var scanCode = MapVirtualKeyExA((uint) value, MAPVK_VK_TO_VSC, _currrentKeyboardLayout);

                var newLParam = FakeKeyLParam(scanCode, vM == VM.KEYDOWN);
            #if DEBUG
                Console.WriteLine($"VK: {vK} -> {value}, lParam: {newLParam}");
            #endif
                SendMessage(_appToHook, message, value, newLParam);
            }

            return 0;
        }
        #endregion

        #region Helper Methods
        private static bool IsHexValue(string str) => str.StartsWith("0x");

        private static nint StrToNint(string str)
        {
            if(IsHexValue(str)) return nint.Parse(str[2..], NumberStyles.HexNumber);

            if(Enum.IsDefined(typeof(VK), str))
            {
                return(nint) (uint) Enum.Parse(typeof(VK), str, true);
            }
            
            return char.Parse(str);
        }

        private static int GetXButtonVirtualKey(nint wParam) => (int) VK.MBUTTON + HIWORD(wParam);

        private static bool IsMWheelGoingUp(nint wParam) => (short) HIWORD(wParam) > 0;

        private static nint FakeKeyLParam(nint scanCode, bool isKeyDown)
        {
            const int repeat = 1;

            var scancode = (int) scanCode << 16;
            var downOrUpFlag = isKeyDown ? 0 : 3 << 30;

            return repeat + scancode + downOrUpFlag;
        }
        #endregion
    }
}
