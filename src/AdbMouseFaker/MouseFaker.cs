using AdbSender;
using AdbSender.Models;

namespace AdbMouseFaker;

public class MouseFaker : IMouseFaker
{
    private readonly IAdbClient _adbClient;
    // TODO: Do I want to provide a way to get the /dev/input's automatically? ( getevent -i could help )
    private readonly string _mouseInputDevice;

    private int _currentMtSlot = 0;

    public MouseFaker(IAdbClient adbClient, string mouseInputDevice)
    {
        _adbClient = adbClient;
        _mouseInputDevice = mouseInputDevice;
    }

    ~MouseFaker() => this.Dispose(false);

    public Tap Tap(int x, int y)
    {
        if(++_currentMtSlot != 1)
        {
            this.Send_ABS_MT_SLOT(_currentMtSlot);
            // FIX: Not sure if it does matter sending MT_SLOT 1 at the start.
        }

        this.Send_ABS_MT_TRACKING_ID(_currentMtSlot);
        this.Send_EV_KEY(_mouseInputDevice, BTN_TOUCH, keyDown: true);

        Tap to = new(_currentMtSlot, x, y);
        Tap from = new(-1, to.X - 1, to.Y - 1); // I can't really know the values of the previous last tap.
        this.Send_ABS_MT_POSITION(in from, in to);

        return to;
    }

    public void TapMove(in Tap from, in Tap to)
    {
        this.ChangeSlot(in from);
        this.Send_ABS_MT_POSITION(in from, in to);
    }

    public void TapRelease(in Tap tap)
    {
        this.ChangeSlot(in tap);
        this.Send_ABS_MT_TRACKING_ID(-1);
        this.Send_EV_KEY(_mouseInputDevice, BTN_TOUCH, keyDown: false);
        this.Send_SYN_REPORT(_mouseInputDevice);

        _currentMtSlot--;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if(disposing)
        {
        }

        while(_currentMtSlot != 0)
        {
            var tap = new Tap(_currentMtSlot, default, default);
            this.TapRelease(in tap);
        }
    }

    private void Send_ABS_MT_SLOT(int slot)
    {
        InputEvent mtSlot = new((int)EV_ABS, (int)ABS_MT_SLOT, slot);
        _adbClient.ExecuteSendEvent(_mouseInputDevice, in mtSlot);
    }

    private void Send_ABS_MT_TRACKING_ID(int id)
    {
        InputEvent mtTrackingId = new((int)EV_ABS, (int)ABS_MT_TRACKING_ID, id);
        _adbClient.ExecuteSendEvent(_mouseInputDevice, in mtTrackingId);
    }

    private void Send_EV_KEY(ReadOnlySpan<char> input, KeyButton keyCode, bool keyDown)
    {
        InputEvent evKey = new((int)EV_KEY, (int)keyCode, keyDown ? 1 : 0);
        _adbClient.ExecuteSendEvent(input, in evKey);
    }

    private void Send_ABS_MT_POSITION(in Tap from, in Tap to)
    {
        var (_, lastX, lastY) = from; var (_, x, y) = to;

        if(x == lastX && y == lastY) return;

        if(x != lastX)
        {
            InputEvent xEvent = new((int)EV_ABS, (int)ABS_MT_POSITION_X, x);
            _adbClient.ExecuteSendEvent(_mouseInputDevice, xEvent);
        }

        if(y != lastY)
        {
            InputEvent yEvent = new((int)EV_ABS, (int)ABS_MT_POSITION_Y, y);
            _adbClient.ExecuteSendEvent(_mouseInputDevice, yEvent);
        }

        this.Send_SYN_REPORT(_mouseInputDevice);
    }

    private void Send_SYN_REPORT(ReadOnlySpan<char> input)
    {
        InputEvent synReport = new((int)EV_SYN, (int)SYN_REPORT, 0);
        _adbClient.ExecuteSendEvent(input, in synReport);
    }

    private void ChangeSlot(in Tap tap)
    {
        var id = tap.Id;

        if(_currentMtSlot != id)
        {
            this.Send_ABS_MT_SLOT(id);
        }
    }
}
