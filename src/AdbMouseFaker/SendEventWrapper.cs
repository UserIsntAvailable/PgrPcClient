using System.Net;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;

namespace AdbMouseFaker
{
    public class SendEventWrapper : ISendEventWrapper
    {
        private readonly IAdbClient _client;
        private readonly DeviceData _device;
        private readonly InfoReceiver _outputReceiver = new();

        public SendEventWrapper(IAdbClient client, string deviceName, DnsEndPoint endPoint)
        {
            _client = client;
            _device = _client.ConnectToDevice(deviceName, endPoint);
        }

        public void Send(string deviceSource, int type, int code, int value)
        {
            _client.ExecuteRemoteCommand($"sendevent {deviceSource} {type} {code} {value}", _device, _outputReceiver);
        }
    }
}
