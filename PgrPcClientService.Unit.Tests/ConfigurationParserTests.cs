using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Win32Api;
using Xunit;

namespace PgrPcClientService.Unit.Tests
{
    public class ConfigurationParserTests
    {
        private readonly ConfigurationParser _sut = new();

        [Fact]
        public void GetBinds_ShouldReturnBinds_WhenConfigIsValid()
        {
            var (config, actual) = CreateFakeData();

            var expected = _sut.GetBinds(config);

            expected.Should().BeEquivalentTo(actual);

            static (IConfiguration, IDictionary<nint, nint>) CreateFakeData()
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
}
