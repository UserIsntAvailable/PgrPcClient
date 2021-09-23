using FluentAssertions;
using PgrPcClient;
using Xunit;

namespace PgrPcClientService.Unit.Tests
{
    public class ConfigurationParserTests
    {
        private readonly ConfigurationParser _sut = new();

        [Fact]
        public void GetBinds_ShouldReturnBinds_WhenConfigIsValid()
        {
            var (config, actual) = FakeDataFactory.CreateConfigFakeData();

            var expected = _sut.GetBinds(config);

            expected.Should().BeEquivalentTo(actual);
        }
    }
}
