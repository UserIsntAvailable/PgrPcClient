using System.Collections.Generic;
using System.Linq;
using WindowsAppOverlay;
using Microsoft.Extensions.Configuration;
using PgrPcClientService;
using static Win32Api.Window;

// TODO - Create a better 'background service'
while(true)
{
    // Using MuMu CN, it should work with other Emulators, just find the Window name that is handling the emulator
    var pgrHandle = FindWindowsWithText("PGR - Netease Emulator").FirstOrDefault();

    if(pgrHandle != 0)
    {
        PGRMessageHandler pgrMessageHandler = new(ParseConfig(pgrHandle));
        AppOverlay overlay = new(pgrMessageHandler, "PGRPcSimulatorClass");
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
