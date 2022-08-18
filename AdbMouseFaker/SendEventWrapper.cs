using System.Net;
using SharpAdbClient;

namespace AdbMouseFaker
{
    public class SendEventWrapper : ISendEventWrapper
    {
        private readonly AdbClient _client;
        private readonly DeviceData _device;

        public SendEventWrapper(AdbClient client, DnsEndPoint endPoint)
        {
            _client = client;
            _device = _client.ConnectToDevice($"{endPoint.Host}:{endPoint.Port}", endPoint);
        }

        public void Send(string deviceSource, int type, int code, int value)
        {
            _client.ExecuteRemoteCommand($"sendevent {deviceSource} {type} {code} {value}", _device, null);
        }
    }
}
