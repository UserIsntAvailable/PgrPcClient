using System;
using System.Linq;
using AutoFixture;
using NSubstitute;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

namespace AdbMouseFaker.Unit.Tests;

public class MouseFakerTests : IDisposable
{
    private const string MOUSE_INPUT_DEVICE = "/dev/input/mouse";

    private readonly IMouseFaker _sut;
    private readonly AdbClientUnrollRefs _adbClient;
    private readonly IFixture _fixture = new Fixture();

    public MouseFakerTests()
    {
        _adbClient = Substitute.For<AdbClientUnrollRefs>();
        _sut = new MouseFaker(_adbClient, MOUSE_INPUT_DEVICE);
    }

    [Fact]
    public void Tap_ShouldCreateTapStruct_WhenValidData()
    {
        var expectedX = _fixture.Create<int>();
        var expectedY = _fixture.Create<int>();

        var actual = _sut.Tap(expectedX, expectedY);

        actual.Id.Should().Be(1);
        expectedX.Should().Be(actual.X);
        expectedY.Should().Be(actual.Y);
    }

    [Fact]
    public void Tap_ShouldNotSend_ABS_MT_SLOT_WhenFirstTap()
    {
        _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());

        _adbClient.DidNotReceive().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 1);
    }

    [Fact]
    public void Tap_ShouldSend_ABS_MT_SLOT_WhenNotFirstTap()
    {
        _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());

        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 2);
    }

    [Fact]
    public void Tap_ShouldHandleSingleTouch_WhenTapIsCallOnce()
    {
        var x = _fixture.Create<int>();
        var y = _fixture.Create<int>();

        _sut.Tap(x, y);

        _adbClient.Received(5).ExecuteSendEvent(
            MOUSE_INPUT_DEVICE,
            Arg.Any<EventType>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );


        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, 1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, x);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, y);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }

    [Fact]
    public void Tap_ShouldHandleMultiTouch_WhenTapIsCallMoreThanOnce()
    {
        var firstX = _fixture.Create<int>();
        var firstY = _fixture.Create<int>();
        var secondX = _fixture.Create<int>();
        var secondY = _fixture.Create<int>();

        _sut.Tap(firstX, firstY);
        _sut.Tap(secondX, secondY);

        _adbClient.Received(11).ExecuteSendEvent(
            MOUSE_INPUT_DEVICE,
            Arg.Any<EventType>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );

        // Received.InOrder(
        //     () =>
        //     {
        //         // FIX: This is completely broken
        //         // The call to _adbClient.Received(12) actually shows the good order, but the calls inside here
        //         // are not actually checked by order; If I remove any random "Received" call from here, the test
        //         // still will pass, and if I remove all the calls till the second ABS_MT_SLOT, the "expected"
        //         // output doesn't make any sense. 
        //         // I have 2 solutions for this:
        //         // 1 -> I '_adbClient.ClearReceivedCalls()' until the the calls that I wanna check.
        //         // 2 -> I create my own Received.InOrder method.
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, 1);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 1);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, firstX);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, firstY);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 2);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, 2);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 1);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, secondX);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, secondY);
        //         _adbClient.ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
        //     }
        // );

        // For now I will just assert if the calls were made without order checking.
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 2);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, 2);
        _adbClient.Received(2).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, secondX);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, secondY);
        _adbClient.Received(2).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }

    [Fact]
    public void TapMove_ShouldWork_WhenFromAndToAreDifferent()
    {
        var xMove = _fixture.Create<int>();
        var yMove = _fixture.Create<int>();

        var tap = _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        var move = tap with
        {
            X = xMove, Y = yMove,
        };

        _sut.TapMove(in tap, in move);

        _adbClient.Received(8).ExecuteSendEvent(
            MOUSE_INPUT_DEVICE,
            Arg.Any<EventType>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );

        _adbClient.DidNotReceive().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, Arg.Any<int>());
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, xMove);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, yMove);
        _adbClient.Received().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }

    [Fact]
    public void TapMove_ShouldChangeSlot_WhenDifferentCurrentSlotId()
    {
        var xMove = _fixture.Create<int>();
        var yMove = _fixture.Create<int>();

        var tap = _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        var move = tap with { X = xMove, Y = yMove, };

        _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());

        _sut.TapMove(tap, move);

        _adbClient.Received(15).ExecuteSendEvent(
            MOUSE_INPUT_DEVICE,
            Arg.Any<EventType>(),
            Arg.Any<int>(),
            Arg.Any<int>()
        );

        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, xMove);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, yMove);
        _adbClient.Received().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }

    [Theory]
    [InlineData(1, 2, 1, 3)]
    [InlineData(1, 2, 3, 2)]
    [InlineData(1, 2, 1, 2)]
    // FIX: naming?
    public void TapMove_ShouldIgnore_X_Or_Y_Axes_WhenFromTap_X_Or_Y_AreEqualTo_ToTap_X_Or_Y(
        int xTap,
        int yTap,
        int xMove,
        int yMove)
    {
        var tap = _sut.Tap(xTap, yTap);
        var move = tap with
        {
            X = xMove, Y = yMove,
        };

        _sut.TapMove(tap, move);

        if(xTap != xMove)
        {
            _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_X, xMove);
        }

        if(yTap != yMove)
        {
            _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_POSITION_Y, yMove);
        }

        if(xTap == xMove && yTap == yMove)
        {
            _adbClient.Received(5).ExecuteSendEvent(
                MOUSE_INPUT_DEVICE,
                Arg.Any<EventType>(),
                Arg.Any<int>(),
                Arg.Any<int>()
            );
        }
    }

    [Fact]
    public void TapRelease_ShouldReleaseSingleTap()
    {
        var tap = _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        
        _sut.TapRelease(in tap);
        
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, -1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 0);
        _adbClient.Received().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }
    
    [Fact]
    public void TapRelease_ShouldChangeSlot_WhenDifferentCurrentSlotId()
    {
        var tap = _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        _sut.Tap(_fixture.Create<int>(), _fixture.Create<int>());
        
        _sut.TapRelease(in tap);
        
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_SLOT, 1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_ABS, (int)ABS_MT_TRACKING_ID, -1);
        _adbClient.Received(1).ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_KEY, (int)BTN_TOUCH, 0);
        _adbClient.Received().ExecuteSendEvent(MOUSE_INPUT_DEVICE, EV_SYN, (int)SYN_REPORT, 0);
    }

    public void Dispose()
    {
        _sut.Dispose(); // Not really necessary. ( At least, for now )
    }
}
