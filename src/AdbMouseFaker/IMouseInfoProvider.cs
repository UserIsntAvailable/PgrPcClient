namespace AdbMouseFaker
{
    public interface IMouseInfoProvider
    {
        public (int X, int Y) GetMousePosition();
    }
}
