using System;
using System.Threading;

namespace AdbMouseFaker
{
    public class MouseFaker : IMouseFaker
    {
        private const int EV_SYN = 0x00,
                          EV_KEY = 0x01,
                          EV_ABS = 0x03,
                          SYN_REPORT = 0x00,
                          BTN_TOUCH = 0x14A,
                          ABS_MT_POSITION_X = 0x35,
                          ABS_MT_POSITION_Y = 0x36,
                          ABS_MT_TRACKING_ID = 0x39,
                          DEFAULT_TRACKING_ID = 1,
                          RELEASE_TRACKING_ID = -1;

        private readonly IMouseInfoProvider _mouseInfoProvider;
        private readonly ISendEventWrapper _sendEventWrapper;
        private readonly string _deviceMouseInput;
        private readonly ManualResetEvent _suspendEvent = new(false);

        private bool _isDragging;

        public MouseFaker(
            ISendEventWrapper sendEventWrapper,
            IMouseInfoProvider mouseInfoProvider,
            string deviceMouseInput)
        {
            _sendEventWrapper = sendEventWrapper;
            _mouseInfoProvider = mouseInfoProvider;
            _deviceMouseInput = deviceMouseInput;

            this.CreateDraggingModeThread();
        }

        /* BUG - IsDragging releases the mouse cursor before the cursor is in the last position.
         *       That breaks the cursor and the cursor never releases.
         */
        public bool IsDragging
        {
            get => _isDragging;
            set
            {
                if(value)
                {
                    if(!_isDragging)
                    {
                        var (x, y) = _mouseInfoProvider.GetMousePosition();

                        this.ClipMouse(x, y);
                        _suspendEvent.Set();
                    }
                }
                else
                {
                    if(_isDragging)
                    {
                        _suspendEvent.Reset();
                        this.ReleaseMouse();
                    }
                }

                _isDragging = value;
            }
        }

        public void Click(int x, int y)
        {
            if(_isDragging)
                throw new InvalidOperationException($"You can not send a click event while {this.IsDragging} is true.");

            this.ClipMouse(x, y);
            this.ReleaseMouse();
        }

        private void CreateDraggingModeThread()
        {
            new Thread(
                () =>
                {
                    int lastX = 0,
                        lastY = 0;

                    // I will just let the Process to destroy the Thread.
                    while(true)
                    {
                        _suspendEvent.WaitOne(Timeout.Infinite);
                        
                        var (x, y) = _mouseInfoProvider.GetMousePosition();

                        this.MoveMouse(x, y, lastX, lastY);

                        lastX = x;
                        lastY = y;
                    }
                    // ReSharper disable once FunctionNeverReturns
                }
            )
            {
                IsBackground = true, Name = "DraggingModeThread",
            }.Start();
        }

        private void ClipMouse(int x, int y)
        {
            _sendEventWrapper.Send(_deviceMouseInput, EV_ABS, ABS_MT_TRACKING_ID, DEFAULT_TRACKING_ID);
            _sendEventWrapper.Send(_deviceMouseInput, EV_KEY, BTN_TOUCH, 1);

            this.MoveMouse(x, y, 0, 0);
        }

        private void MoveMouse(int x, int y, int lastX, int lastY)
        {
            if(x == lastX && y == lastY) return;

            if(x != lastX) _sendEventWrapper.Send(_deviceMouseInput, EV_ABS, ABS_MT_POSITION_X, x);

            if(y != lastY) _sendEventWrapper.Send(_deviceMouseInput, EV_ABS, ABS_MT_POSITION_Y, y);

            _sendEventWrapper.Send(_deviceMouseInput, EV_SYN, SYN_REPORT, 0);
        }

        private void ReleaseMouse()
        {
            _sendEventWrapper.Send(_deviceMouseInput, EV_ABS, ABS_MT_TRACKING_ID, RELEASE_TRACKING_ID);
            _sendEventWrapper.Send(_deviceMouseInput, EV_KEY, BTN_TOUCH, 0);
            _sendEventWrapper.Send(_deviceMouseInput, EV_SYN, SYN_REPORT, 0);
        }
    }
}
