using FluentAssertions;
using Xunit;
using static Win32Api.Message;

namespace WindowsAppOverlay.Unit.Tests
{
    public class MessageHandlerTests
    {
        private readonly IMessageHandler _sut = new MessageHandler();

        [Fact]
        public void TryGetMessageDelegate_ShouldReturnTrue_WhenMessageIsInTheDictionary()
        {
            var expected = _sut.TryGetMessageDelegate((uint) WM.DESTROY, out _);

            expected.Should().BeTrue();
        }
    }
}
