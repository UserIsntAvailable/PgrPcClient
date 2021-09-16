using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Win32Api;

namespace PgrPcClientService.Unit.Tests
{
    public static class FakeDataFactory
    {
        public static (IConfiguration config, IDictionary<nint, nint> configBinds) CreateConfigFakeData()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string>("GameBindings:ATK", "LBUTTON"),
                    new KeyValuePair<string, string>("GameBindings:Jump", "SPACE"),
                    new KeyValuePair<string, string>("OverlayBindings:SPACE", "0x0E"),
                    new KeyValuePair<string, string>("OverlayBindings:RBUTTON", "ATK"),
                }
            ).Build();

            Dictionary<nint, nint> dict = new()
            {
                {(nint) Keyboard.VK.SPACE, int.Parse("0E", NumberStyles.HexNumber)},
                {(nint) Keyboard.VK.RBUTTON, (nint) Keyboard.VK.LBUTTON},
            };

            return(config, dict);
        }
    }
}
