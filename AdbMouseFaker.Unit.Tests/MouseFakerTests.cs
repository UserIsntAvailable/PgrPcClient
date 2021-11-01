using AutoFixture;
using NSubstitute;
using Xunit;
using static AdbMouseFaker.SendEventConstant;

namespace AdbMouseFaker.Unit.Tests
{
    public class MouseFakerTests
    {
        private const string TEST_DEVICE_MOUSE_INPUT = "TEST/DEV/";

        private readonly IMouseFaker _sut;
        private readonly IFixture _fixture = new Fixture();
        private readonly ISendEventWrapper _sendEventWrapper = Substitute.For<ISendEventWrapper>();
        private readonly IMouseInfoProvider _mouseInfoProvider = Substitute.For<IMouseInfoProvider>();

        public MouseFakerTests()
        {
            _sut = new MouseFaker(_sendEventWrapper, _mouseInfoProvider, TEST_DEVICE_MOUSE_INPUT);
        }

        [Fact]
        public void Click_ShouldWorkOnSingleTouch_WhenXAndYAreValid()
        {
            var actualX = _fixture.Create<int>();
            var actualY = _fixture.Create<int>();

            _sut.Click(actualX, actualY);

            Received.InOrder(
                () =>
                {
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_TRACKING_ID, DEFAULT_TRACKING_ID);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_KEY, BTN_TOUCH, 1);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_X, actualX);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_Y, actualY);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_SYN, SYN_REPORT, 0);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_TRACKING_ID, RELEASE_TRACKING_ID);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_KEY, BTN_TOUCH, 0);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_SYN, SYN_REPORT, 0);
                }
            );
        }
        
        [Fact]
        public void IsDragging_ShouldWork_WhenSetToTrueAndBackToFalse()
        {
            var firstActualX = _fixture.Create<int>();
            var secondActualX = _fixture.Create<int>();
            var firstActualY = _fixture.Create<int>();
            var secondActualY = _fixture.Create<int>();

            _mouseInfoProvider.GetMousePosition().Returns(
                (firstActualX, firstActualY),
                (secondActualX, secondActualY)
            );

            _sut.IsDragging = true;
            _sut.IsDragging = false;

            Received.InOrder(
                () =>
                {
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_TRACKING_ID, DEFAULT_TRACKING_ID);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_KEY, BTN_TOUCH, 1);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_X, firstActualX);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_Y, firstActualY);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_SYN, SYN_REPORT, 0);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_X, secondActualX);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_POSITION_Y, secondActualY);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_SYN, SYN_REPORT, 0);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_ABS, ABS_MT_TRACKING_ID, RELEASE_TRACKING_ID);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_KEY, BTN_TOUCH, 0);
                    _sendEventWrapper.Send(TEST_DEVICE_MOUSE_INPUT, EV_SYN, SYN_REPORT, 0);
                }
            );
        }

        [Fact]
        public void IsDragging_ShouldDoNothing_WhenSetToItsOwnValue()
        {
            _mouseInfoProvider.GetMousePosition().Returns((_fixture.Create<int>(), _fixture.Create<int>()));

            _sut.IsDragging = true;
            _sendEventWrapper.ClearReceivedCalls();
            _sut.IsDragging = true;
            _sendEventWrapper.DidNotReceiveWithAnyArgs().Send(default, default, default, default);
            
            _sut.IsDragging = false;
            _sendEventWrapper.ClearReceivedCalls();
            _sut.IsDragging = false;
            _sendEventWrapper.DidNotReceiveWithAnyArgs().Send(default, default, default, default);
        }
    }
}
