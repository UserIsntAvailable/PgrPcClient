using System.Linq;
using System.Net;
using SharpAdbClient;

namespace AdbMouseFaker
{
    public static class IAdbClientExtensions
    {
        public static DeviceData ConnectToDevice(this IAdbClient client, string deviceName, DnsEndPoint endPoint)
        {
            client.Connect(endPoint);
            var devices = client.GetDevices();

            return devices.First(n => n.Serial == deviceName);
        }
    }
}
