namespace AdbMouseFaker;

public interface IMouseFaker : IDisposable
{
    public Tap Tap(int x, int y);

    public void TapMove(in Tap from, in Tap to);

    public void TapRelease(in Tap tap);
}
