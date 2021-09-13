using System.Collections.Generic;
using System.Linq;
using System.Net;
using WindowsAppOverlay;
using AdbMouseFaker;
using Microsoft.Extensions.Configuration;
using PgrPcClientService;
using SharpAdbClient;
using static Win32Api.Mouse;
using static Win32Api.Window;

// TODO - Organize the appsettings.json better
// TODO - Create a better 'background service'
// TODO - Create an ADB server if not started
while(true)
{
    // Using MuMu CN, it should work with other Emulators, just find the Window name that is handling the emulator
    var pgrHandle = FindWindowsWithText("PGR - Netease Emulator").FirstOrDefault();

    if(pgrHandle != 0)
    {
        var config = ParseConfig(pgrHandle);

        var host = config["DeviceHost"];
        var port = int.Parse(config["DevicePort"]);
        var deviceName = $"{host}:{port}";
        var deviceInput = config["DeviceInput"];

        DnsEndPoint endPoint = new(host, port);
        SendEventWrapper sendEventWrapper = new(new AdbClient(), deviceName, endPoint);
        MouseFaker mouseFaker = new(sendEventWrapper, new WindowsMouseInfoProvider(), deviceInput);

        PGRMessageHandler pgrMessageHandler = new(mouseFaker, config);
        AppOverlay overlay = new(pgrMessageHandler, config["AppClassName"]);
        overlay.Run();
    }
}

static IConfiguration ParseConfig(nint pgrHandle)
{
    return new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                     .AddInMemoryCollection(
                                         new[] {new KeyValuePair<string, string>("AppToHook", pgrHandle.ToString()),}
                                     ).Build();
}

class WindowsMouseInfoProvider : IMouseInfoProvider
{
    public (int X, int Y) GetMousePosition()
    {
        GetCursorPos(out var pos);

        return(pos.X, pos.Y);
    }
}
