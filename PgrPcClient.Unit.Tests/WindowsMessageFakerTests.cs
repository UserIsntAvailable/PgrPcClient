#if WINDOWS
namespace PgrPcClient.Unit.Tests
{
    public class WindowsMessageFakerTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void KeyMessage_ShouldCallMessageSenderDirectly_WhenWParamIsNotBind()
        {
            var actualIsKeyDown = _fixture.Create<bool>();
            var actualWParam = _fixture.Create<int>();
            var actualLParam = _fixture.Create<int>();

            var actualAppHWnd = _fixture.Create<int>();
            var (_, actualBinds) = FakeDataFactory.CreateConfigFakeData();

            var sut = new WindowsMessageFaker(
                actualAppHWnd,
                actualBinds,
                (expectedHWnd, expectedMessage, expectedWParam, expectedLParam) =>
                {
                    expectedHWnd.Should().Be((nint)actualAppHWnd);
                    expectedMessage.Should().Be((uint) (actualIsKeyDown ? WM.KEYDOWN : WM.KEYUP));
                    expectedWParam.Should().Be((nint)actualWParam);
                    expectedLParam.Should().Be((nint)actualLParam);

                    return 0;
                }
            );

            sut.KeyMessage(actualIsKeyDown, actualWParam, actualLParam);
        }

        [Fact]
        public void KeyMessage_ShouldCallVirtualKeyMessage_WhenWParamIsBind()
        {
            var actualIsKeyDown = _fixture.Create<bool>();

            var actualAppHWnd = _fixture.Create<int>();
            var (_, actualBinds) = FakeDataFactory.CreateConfigFakeData();

            var actualLParam = _fixture.Create<int>();
            var notActualWParam = actualBinds.Keys.First();

            var sut = new WindowsMessageFaker(
                actualAppHWnd,
                actualBinds,
                (expectedHWnd, expectedMessage, expectedWParam, expectedLParam) =>
                {
                    expectedHWnd.Should().Be((nint)actualAppHWnd);
                    expectedMessage.Should().Be((uint) (actualIsKeyDown ? WM.KEYDOWN : WM.KEYUP));
                    expectedWParam.Should().NotBe(notActualWParam);
                    expectedLParam.Should().NotBe((nint)actualLParam);

                    return 0;
                }
            );

            sut.KeyMessage(actualIsKeyDown, notActualWParam, actualLParam);
        }
    }
}
#endif
