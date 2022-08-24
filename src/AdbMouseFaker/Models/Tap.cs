namespace AdbMouseFaker.Models;

public readonly ref struct Tap
{
    public Tap()
    {
        this.Id = -1;
        this.X = default;
        this.Y = default;
    }

    internal Tap(int id, int x, int y)
    {
        this.Id = id;
        this.X = x;
        this.Y = y;
    }

    public int Id { get; }

    public int X { get; init; }

    public int Y { get; init; }
    
    public void Deconstruct(out int id, out int x, out int y)
    {
        id = this.Id;
        x = this.X;
        y = this.Y;
    }
}
