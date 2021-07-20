using System;
using System.Linq;
using System.Net;
using System.Threading;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;

namespace AdbMouseFaker
{
    public class MouseFaker
    {
        private readonly IAdbClient _client;
        private readonly IShellOutputReceiver _outputReceiver = new InfoReceiver();
        private readonly ManualResetEvent _suspendEvent = new ManualResetEvent(false);

        private DeviceData _device;

        private bool _isCameraModeOn;
        /// <summary>
        /// Sets if the camera mode is active. ( Simulates a 3D camera. )
        /// </summary>
        public bool IsCameraModeOn
        {
            get => _isCameraModeOn;
            set
            {
                _isCameraModeOn = value;

                if(_isCameraModeOn)
                {
                    _suspendEvent.Set();
                }
                else
                {
                    _suspendEvent.Reset();
                }
            }
        }

        public MouseFaker(IAdbClient client, string deviceName, DnsEndPoint endPoint)
        {
            _client = client;

            this.ConnectToDevice(deviceName, endPoint);
            this.ConfigureCameraModeThread();
        }

        /// <summary>
        /// Sends a click message to the connected device.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public void MouseClick(int x, int y)
        {
            _client.ExecuteRemoteCommand($"input tap {x} {y}", _device, _outputReceiver);
        }

        private void ConnectToDevice(string deviceName, DnsEndPoint endPoint)
        {
            _client.Connect(endPoint);
            var devices = _client.GetDevices();

            _device = devices.First(n => n.Serial == deviceName);
        }

        private void ConfigureCameraModeThread()
        {
            new Thread(
                () =>
                {
                    // I will just let the Process to destroy the Thread.
                    while(true)
                    {
                        _suspendEvent.WaitOne(Timeout.Infinite);

                        // TODO - Get info about mouse position
                    }
                }
            )
            {
                IsBackground = true, Name = "CameraModeThread",
            }.Start();
        }
    }
}
