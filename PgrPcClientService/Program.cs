using System.Collections.Generic;
using System.Linq;
using System.Net;
using AdbMouseFaker;
using Microsoft.Extensions.Configuration;
using PgrPcClientService;
using SharpAdbClient;
using Win32Api;
using WindowsAppOverlay;
using static Win32Api.Mouse;
using static Win32Api.Window;
using ConfigurationParser = PgrPcClientService.ConfigurationParser;

/*
 * TODO - Organize the appsettings.json better
 * TODO - Implement auto reloading of the appsettings.json
 * TODO - Create a better 'background service'
 * TODO - Create an ADB server if not started
 */
while(true)
{
    // Using MuMu CN, it should work with other Emulators, just find the Window name that is handling the emulator
    var pgrHandle = FindWindowsWithText("PGR - Netease Emulator").FirstOrDefault();

    if(pgrHandle != 0)
    {
        var config = SetupConfig(pgrHandle);

        var host = config["DeviceHost"];
        var port = int.Parse(config["DevicePort"]);
        var deviceName = $"{host}:{port}";
        var deviceInput = config["DeviceInput"];

        DnsEndPoint endPoint = new(host, port);
        SendEventWrapper sendEventWrapper = new(new AdbClient(), deviceName, endPoint);
        MouseFaker mouseFaker = new(sendEventWrapper, new WindowsMouseInfoProvider(), deviceInput);

        ConfigurationParser configParser = new();
        WindowsMessageFaker messageFaker = new(
            nint.Parse(config["PgrAppHWnd"]),
            configParser.GetBinds(config),
            Message.SendMessage
        );

        MessageHandler messageHandler = new();
        // I'm not sure if I wanna keep this as a 'class'; there should be a better alternative. 
        new HandleMessageMapper(messageHandler, messageFaker, mouseFaker);
        AppOverlay overlay = new(messageHandler, config["AppClassName"]);
        overlay.Run();
    }
}

static IConfiguration SetupConfig(nint pgrHandle)
{
    return new ConfigurationBuilder().AddJsonFile("appsettings.json").AddInMemoryCollection(
        new[] { new KeyValuePair<string, string>("PgrAppHWnd", pgrHandle.ToString()), }
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
