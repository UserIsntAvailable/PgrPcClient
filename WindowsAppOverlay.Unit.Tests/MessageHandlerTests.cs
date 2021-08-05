using FluentAssertions;
using static Win32Api.Message;
using Xunit;

namespace WindowsAppOverlay.Unit.Tests
{
    public class MessageHandlerTests
    {
        private readonly IMessageHandler _sut = new MessageHandler();

        [Fact]
        public void TryGetMessageDelegate_ShouldReturnTrue_WhenMessageIsInTheDictionary()
        {
            var expected = _sut.TryGetMessageDelegate((uint) VM.DESTROY, out _);

            expected.Should().BeTrue();
        }
    }
}
