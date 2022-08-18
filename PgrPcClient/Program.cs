using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AdbMouseFaker;
using Microsoft.Extensions.Configuration;
using PgrPcClient;
using PgrPcClient.Extensions;
using SharpAdbClient;
using WindowsAppOverlay;
using static Win32Api.Mouse;
using static Win32Api.Window;
using static Win32Api.Message;
using ConfigurationParser = PgrPcClient.ConfigurationParser;

/*
 * TODO - Implement auto reloading of the appsettings.json
 * TODO - Create an ADB server if not started
 */
Console.WriteLine("Waiting for PGR process");

nint pgrHandle = 0;

while(pgrHandle == 0)
{
    /*
     * Using MuMu CN, it should work with other Emulators,
     * just find the Window name that is handling the emulator
     * and don't forget to modify EmulatorInfo in the appsettings.json
     */
    pgrHandle = FindWindowsWithText("PGR - Netease Emulator").FirstOrDefault();
}

var message = $"PGR process found ( {pgrHandle} )";
Console.WriteLine(message);
Console.WriteLine(new string('-', message.Length));

var config = SetupConfig(pgrHandle);

var host = config["EmulatorInfo:DeviceHost"];
var port = int.Parse(config["EmulatorInfo:DevicePort"]);
var deviceInput = config["EmulatorInfo:DeviceInput"];

DnsEndPoint endPoint = new(host, port);
SendEventWrapper sendEventWrapper = new(new AdbClient(), endPoint);
MouseFaker mouseFaker = new(sendEventWrapper, new WindowsMouseInfoProvider(), deviceInput);

MessageHandler messageHandler = new();
messageHandler.Map(
    WM.CREATE,
    // TODO - Refactor ( I will parse the IN-GAME config file directly )
    (hWnd, _, _) =>
    {
        KeyBindsWindow.CreateKeyBindsChildWindow(
            hWnd,
            GetKeysToDrawOnOverlay(config),
            (int.Parse(config["KeyBindsChildWindowSettings:xStartPosition"]),
             int.Parse(config["KeyBindsChildWindowSettings:yStartPosition"])),
             int.Parse(config["KeyBindsChildWindowSettings:Padding"])
        );

        return 0;
    }
);

ConfigurationParser configParser = new();
WindowsMessageFaker messageFaker = new(nint.Parse(config["PgrAppHWnd"]), configParser.GetBinds(config), SendMessage);
new HandleMessageMapper(messageHandler, messageFaker, mouseFaker); // I'm not sure if I wanna keep this as a 'class'; there should be a better alternative. 
AppOverlay overlay = new(messageHandler, config["OverlayInfo:AppClassName"]);
overlay.Run();

static IConfiguration SetupConfig(nint pgrHandle) =>
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddInMemoryCollection(new[] { new KeyValuePair<string, string>("PgrAppHWnd", pgrHandle.ToString()), })
        .Build();

static IEnumerable<string> GetKeysToDrawOnOverlay(IConfiguration config) =>
    config.GetSection("OverlayInfo:Bindings")
          .GetChildren()
              .Where(child => child.Value.StartsWith("Signal"))
              .OrderBy(child => child.Value)
              .Select(child =>
              {
                  // TODO - Create string parser
                  var value = child.Key;

                  if(value.Length == 1) return value;

                  if(value.IsHexValue())
                  {
                     return value == "0x0E" ? "WU" : "WD";
                  }

                  if(value.StartsWith("XB"))
                  {
                      return$"XB{value[^1]}";
                  }

                  return value[..2];
              });


internal class WindowsMouseInfoProvider : IMouseInfoProvider
{
    public (int X, int Y) GetMousePosition()
    {
        GetCursorPos(out var pos);

        return(pos.X, pos.Y);
    }
}
