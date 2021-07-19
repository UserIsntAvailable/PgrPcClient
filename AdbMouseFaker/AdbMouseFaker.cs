using System.Linq;
using System.Net;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;

namespace AdbMouseFaker
{
    public class AdbMouseFaker
    {
        private readonly IAdbClient _client;
        private readonly IShellOutputReceiver _outputReceiver = new InfoReceiver();

        private DeviceData _device;

        public AdbMouseFaker(IAdbClient client, string deviceName, DnsEndPoint endPoint)
        {
            _client = client;

            this.ConnectToDevice(deviceName, endPoint);
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
    }
}
