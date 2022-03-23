namespace AdbMouseFaker
{
    public interface IMouseFaker
    {
        public bool IsDragging { get; set; }

        public void Click(int x, int y);
    }
}
