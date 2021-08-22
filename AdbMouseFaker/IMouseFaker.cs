namespace AdbMouseFaker
{
    public interface IMouseFaker
    {
        public bool IsCameraModeOn { get; set; }

        public void Click(int x, int y);
    }
}
