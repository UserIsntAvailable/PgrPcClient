using System.Linq;
using System.Net;
using SharpAdbClient;

namespace AdbMouseFaker;

// ReSharper disable once InconsistentNaming
public static class IAdbClientExtensions
{
    public static DeviceData ConnectToDevice(this IAdbClient client, string deviceName, DnsEndPoint endPoint)
    {
        client.Connect(endPoint);
        var devices = client.GetDevices();

        return devices.First(n => n.Serial == deviceName);
    }
}
