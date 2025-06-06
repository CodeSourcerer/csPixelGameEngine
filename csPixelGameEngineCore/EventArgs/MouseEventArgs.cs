namespace csPixelGameEngineCore;

public class MouseEventArgs : System.EventArgs
{
    public int X { get => Position.x; }
    public int Y { get => Position.y; }
    public vi2d Position { get; }

    public MouseEventArgs()
        : base()
    { }

    public MouseEventArgs(int xPos, int yPos)
    {
        Position = new vi2d { x = xPos, y = yPos };
    }
}
