namespace csPixelGameEngineCore;

public class MouseMoveEventArgs : MouseEventArgs
{
    public int XDelta { get; private set; }
    public int YDelta { get; private set; }
    public MouseMoveEventArgs()
        : base()
    { }

    public MouseMoveEventArgs(int x, int y, int xDelta, int yDelta)
        : base(x, y)
    {
        XDelta = xDelta;
        YDelta = yDelta;
    }
}
